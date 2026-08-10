using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 可多选的下拉菜单
    /// </summary>
    [AddComponentMenu("HTFramework/UI/MultiChoice Dropdown", 2)]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public sealed class MultiChoiceDropdown : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler
    {
        internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, ICancelHandler
        {
            [SerializeField]
            private Text m_Text;
            [SerializeField]
            private RectTransform m_RectTransform;
            [SerializeField]
            private Toggle m_Toggle;

            public Text text { get { return m_Text; } set { m_Text = value; } }
            public RectTransform rectTransform { get { return m_RectTransform; } set { m_RectTransform = value; } }
            public Toggle toggle { get { return m_Toggle; } set { m_Toggle = value; } }
            public int Index { get; set; }

            public virtual void OnPointerEnter(PointerEventData eventData)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
            public virtual void OnCancel(BaseEventData eventData)
            {
                MultiChoiceDropdown dropdown = GetComponentInParent<MultiChoiceDropdown>();
                if (dropdown)
                    dropdown.Hide();
            }
        }
        [Serializable]
        public class DropdownEvent : UnityEvent<List<int>> { }

        private const int HighSortingLayer = 30000;

        [SerializeField]
        private RectTransform m_Template;
        [SerializeField]
        private Text m_CaptionText;
        [Space]
        [SerializeField]
        private Text m_ItemText;
        [Space]
        [SerializeField]
        private List<int> m_Values = new List<int>();
        [SerializeField]
        private string m_Separator = ",";
        [Space]
        [SerializeField]
        private List<string> m_Options = new List<string>();
        [Space]
        [SerializeField]
        private DropdownEvent m_OnValueChanged = new DropdownEvent();

        public RectTransform template { get { return m_Template; } set { m_Template = value; RefreshShownValue(); } }
        public Text captionText { get { return m_CaptionText; } set { m_CaptionText = value; RefreshShownValue(); } }
        public Text itemText { get { return m_ItemText; } set { m_ItemText = value; RefreshShownValue(); } }
        public List<string> options { get { return m_Options; } set { m_Options = value; RefreshShownValue(); } }
        public DropdownEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        private GameObject _dropdownList;
        private GameObject _blocker;
        private List<DropdownItem> _items = new List<DropdownItem>();
        private bool _validTemplate = false;
        private HashSet<int> _duplicatesHashSet = new HashSet<int>();
        private List<int> _duplicatesList = new List<int>();

        protected override void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            if (m_Template)
                m_Template.gameObject.SetActive(false);
        }
        protected override void Start()
        {
            base.Start();

            RefreshShownValue();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
                return;

            RefreshShownValue();
        }
#endif
        protected override void OnDisable()
        {
            DestroyDropdownList();

            if (_blocker != null)
                DestroyBlocker(_blocker);
            _blocker = null;

            base.OnDisable();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Show();
        }
        public void OnSubmit(BaseEventData eventData)
        {
            Show();
        }
        public void OnCancel(BaseEventData eventData)
        {
            Hide();
        }

        /// <summary>
        /// 显示下拉菜单
        /// </summary>
        public void Show()
        {
            if (!IsActive() || !IsInteractable() || _dropdownList != null)
                return;

            List<Canvas> list = ListPool<Canvas>.Get();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count == 0)
                return;

            int listCount = list.Count;
            Canvas rootCanvas = list[listCount - 1];
            for (int i = 0; i < listCount; i++)
            {
                if (list[i].isRootCanvas || list[i].overrideSorting)
                {
                    rootCanvas = list[i];
                    break;
                }
            }

            ListPool<Canvas>.Release(list);

            if (!_validTemplate)
            {
                SetupTemplate(rootCanvas);
                if (!_validTemplate)
                    return;
            }

            m_Template.gameObject.SetActive(true);

            _dropdownList = CreateDropdownList(m_Template.gameObject);
            _dropdownList.name = "MultiChoice Dropdown List";
            _dropdownList.SetActive(true);

            RectTransform dropdownRectTransform = _dropdownList.transform as RectTransform;
            dropdownRectTransform.SetParent(m_Template.transform.parent, false);

            DropdownItem itemTemplate = _dropdownList.GetComponentInChildren<DropdownItem>();
            GameObject content = itemTemplate.rectTransform.parent.gameObject;
            RectTransform contentRectTransform = content.transform as RectTransform;
            itemTemplate.rectTransform.gameObject.SetActive(true);

            Rect dropdownContentRect = contentRectTransform.rect;
            Rect itemTemplateRect = itemTemplate.rectTransform.rect;

            Vector2 offsetMin = itemTemplateRect.min - dropdownContentRect.min + (Vector2)itemTemplate.rectTransform.localPosition;
            Vector2 offsetMax = itemTemplateRect.max - dropdownContentRect.max + (Vector2)itemTemplate.rectTransform.localPosition;
            Vector2 itemSize = itemTemplateRect.size;

            _items.Clear();

            Toggle prev = null;
            var optionsCount = m_Options.Count;
            for (int i = 0; i < optionsCount; i++)
            {
                bool isSelect = m_Values.Contains(i);

                DropdownItem item = AddItem(m_Options[i], itemTemplate, _items);
                if (item == null)
                    continue;

                item.toggle.isOn = isSelect;
                item.toggle.onValueChanged.AddListener(x => OnChangeItem(item));

                if (prev != null)
                {
                    Navigation prevNav = prev.navigation;
                    Navigation toggleNav = item.toggle.navigation;
                    prevNav.mode = Navigation.Mode.Explicit;
                    toggleNav.mode = Navigation.Mode.Explicit;

                    prevNav.selectOnDown = item.toggle;
                    prevNav.selectOnRight = item.toggle;
                    toggleNav.selectOnLeft = prev;
                    toggleNav.selectOnUp = prev;

                    prev.navigation = prevNav;
                    item.toggle.navigation = toggleNav;
                }
                prev = item.toggle;
            }

            Vector2 sizeDelta = contentRectTransform.sizeDelta;
            sizeDelta.y = itemSize.y * _items.Count + offsetMin.y - offsetMax.y;
            contentRectTransform.sizeDelta = sizeDelta;

            float extraSpace = dropdownRectTransform.rect.height - contentRectTransform.rect.height;
            if (extraSpace > 0)
                dropdownRectTransform.sizeDelta = new Vector2(dropdownRectTransform.sizeDelta.x, dropdownRectTransform.sizeDelta.y - extraSpace);

            Vector3[] corners = new Vector3[4];
            dropdownRectTransform.GetWorldCorners(corners);

            RectTransform rootCanvasRectTransform = rootCanvas.transform as RectTransform;
            Rect rootCanvasRect = rootCanvasRectTransform.rect;
            for (int axis = 0; axis < 2; axis++)
            {
                bool outside = false;
                for (int i = 0; i < 4; i++)
                {
                    Vector3 corner = rootCanvasRectTransform.InverseTransformPoint(corners[i]);
                    if ((corner[axis] < rootCanvasRect.min[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.min[axis])) ||
                        (corner[axis] > rootCanvasRect.max[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.max[axis])))
                    {
                        outside = true;
                        break;
                    }
                }
                if (outside)
                    RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, axis, false, false);
            }

            var itemsCount = _items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                RectTransform itemRect = _items[i].rectTransform;
                itemRect.anchorMin = new Vector2(itemRect.anchorMin.x, 0);
                itemRect.anchorMax = new Vector2(itemRect.anchorMax.x, 0);
                itemRect.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, offsetMin.y + itemSize.y * (itemsCount - 1 - i) + itemSize.y * itemRect.pivot.y);
                itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemSize.y);
            }

            m_Template.gameObject.SetActive(false);
            itemTemplate.gameObject.SetActive(false);

            _blocker = CreateBlocker(rootCanvas);
        }
        /// <summary>
        /// 隐藏下拉菜单
        /// </summary>
        public void Hide()
        {
            if (_dropdownList != null)
            {
                if (IsActive())
                    DestroyDropdownList();
            }

            if (_blocker != null)
                DestroyBlocker(_blocker);
            _blocker = null;

            Select();
        }

        /// <summary>
        /// 获取当前选中的选项（索引）
        /// </summary>
        public List<int> GetSelectionIndexs()
        {
            return m_Values;
        }
        /// <summary>
        /// 添加当前选中的选项（索引）
        /// </summary>
        public void AddSelectionIndexs(List<int> indexs, bool sendCallback = true)
        {
            Add(indexs, sendCallback);
        }
        /// <summary>
        /// 添加当前选中的选项（索引）
        /// </summary>
        public void AddSelectionIndex(int index, bool sendCallback = true)
        {
            Add(index, sendCallback);
        }
        /// <summary>
        /// 移除当前选中的选项（索引）
        /// </summary>
        public void RemoveSelectionIndexs(List<int> indexs, bool sendCallback = true)
        {
            Remove(indexs, sendCallback);
        }
        /// <summary>
        /// 移除当前选中的选项（索引）
        /// </summary>
        public void RemoveSelectionIndex(int index, bool sendCallback = true)
        {
            Remove(index, sendCallback);
        }
        /// <summary>
        /// 添加当前选中的选项（内容）
        /// </summary>
        public void AddSelectionValues(List<string> values, bool sendCallback = true)
        {
            if (values == null || values.Count <= 0)
                return;

            List<int> indexs = new List<int>();
            for (int i = 0; i < values.Count; i++)
            {
                int index = m_Options.IndexOf(values[i]);
                if (index >= 0)
                {
                    indexs.Add(index);
                }
            }

            AddSelectionIndexs(indexs, sendCallback);
        }
        /// <summary>
        /// 添加当前选中的选项（内容）
        /// </summary>
        public void AddSelectionValue(string value, bool sendCallback = true)
        {
            int index = m_Options.IndexOf(value);
            if (index >= 0)
            {
                AddSelectionIndex(index, sendCallback);
            }
        }
        /// <summary>
        /// 移除当前选中的选项（内容）
        /// </summary>
        public void RemoveSelectionValues(List<string> values, bool sendCallback = true)
        {
            if (values == null || values.Count <= 0)
                return;

            List<int> indexs = new List<int>();
            for (int i = 0; i < values.Count; i++)
            {
                int index = m_Options.IndexOf(values[i]);
                if (index >= 0)
                {
                    indexs.Add(index);
                }
            }

            RemoveSelectionIndexs(indexs, sendCallback);
        }
        /// <summary>
        /// 移除当前选中的选项（内容）
        /// </summary>
        public void RemoveSelectionValue(string value, bool sendCallback = true)
        {
            int index = m_Options.IndexOf(value);
            if (index >= 0)
            {
                RemoveSelectionIndex(index, sendCallback);
            }
        }
        /// <summary>
        /// 清空当前已选中的选项
        /// </summary>
        public void ClearIndexs(bool sendCallback = true)
        {
            Clear(sendCallback);
        }
        /// <summary>
        /// 刷新显示内容
        /// </summary>
        public void RefreshShownValue()
        {
            string strs = null;
            if (m_Options.Count > 0 && m_Values.Count > 0)
            {
                StringToolkit.BeginConcat();
                for (int i = 0; i < m_Values.Count; i++)
                {
                    int value = Mathf.Clamp(m_Values[i], 0, m_Options.Count - 1);
                    StringToolkit.Concat(m_Options[value]);
                    if (i != m_Values.Count - 1) StringToolkit.Concat(m_Separator);
                }
                strs = StringToolkit.EndConcat();
            }

            if (m_CaptionText)
            {
                m_CaptionText.text = strs;
            }
        }

        /// <summary>
        /// 添加多个可选项
        /// </summary>
        public void AddOptions(List<string> options)
        {
            m_Options.AddRange(options);
            RefreshShownValue();
        }
        /// <summary>
        /// 清空所有可选项
        /// </summary>
        public void ClearOptions()
        {
            m_Options.Clear();
            m_Values.Clear();
            RefreshShownValue();
        }

        private void SetupTemplate(Canvas rootCanvas)
        {
            _validTemplate = false;

            if (!m_Template)
            {
                Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
                return;
            }

            GameObject templateGo = m_Template.gameObject;
            templateGo.SetActive(true);
            Toggle itemToggle = m_Template.GetComponentInChildren<Toggle>();

            _validTemplate = true;
            if (!itemToggle || itemToggle.transform == m_Template)
            {
                _validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", m_Template);
            }
            else if (!(itemToggle.transform.parent is RectTransform))
            {
                _validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", m_Template);
            }
            else if (m_ItemText != null && !m_ItemText.transform.IsChildOf(itemToggle.transform))
            {
                _validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", m_Template);
            }

            if (!_validTemplate)
            {
                templateGo.SetActive(false);
                return;
            }

            DropdownItem item = itemToggle.gameObject.AddComponent<DropdownItem>();
            item.text = m_ItemText;
            item.toggle = itemToggle;
            item.rectTransform = (RectTransform)itemToggle.transform;

            Canvas parentCanvas = null;
            Transform parentTransform = m_Template.parent;
            while (parentTransform != null)
            {
                parentCanvas = parentTransform.GetComponent<Canvas>();
                if (parentCanvas != null)
                    break;

                parentTransform = parentTransform.parent;
            }

            if (!templateGo.TryGetComponent<Canvas>(out _))
            {
                Canvas popupCanvas = templateGo.AddComponent<Canvas>();
                popupCanvas.overrideSorting = true;
                popupCanvas.sortingOrder = HighSortingLayer;
                popupCanvas.sortingLayerID = rootCanvas.sortingLayerID;
            }

            if (parentCanvas != null)
            {
                Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
                for (int i = 0; i < components.Length; i++)
                {
                    Type raycasterType = components[i].GetType();
                    if (templateGo.GetComponent(raycasterType) == null)
                    {
                        templateGo.AddComponent(raycasterType);
                    }
                }
            }
            else
            {
                GetOrAddComponent<GraphicRaycaster>(templateGo);
            }

            templateGo.SetActive(false);

            _validTemplate = true;
        }
        private GameObject CreateDropdownList(GameObject template)
        {
            return Instantiate(template);
        }
        private void DestroyDropdownList()
        {
            _items.Clear();

            if (_dropdownList != null)
                Destroy(_dropdownList);
            _dropdownList = null;
        }
        private DropdownItem AddItem(string data, DropdownItem itemTemplate, List<DropdownItem> items)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            DropdownItem item = Instantiate(itemTemplate);
            item.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);

            item.Index = items.Count;
            item.gameObject.SetActive(true);
            item.gameObject.name = $"Item {items.Count}: {data}";

            if (item.toggle != null)
                item.toggle.isOn = false;

            if (item.text)
                item.text.text = data;

            items.Add(item);
            return item;
        }
        private void OnChangeItem(DropdownItem item)
        {
            if (item.toggle.isOn)
                AddSelectionIndex(item.Index);
            else
                RemoveSelectionIndex(item.Index);

            Hide();
        }

        private GameObject CreateBlocker(Canvas rootCanvas)
        {
            GameObject blocker = new GameObject("Blocker");
            blocker.layer = rootCanvas.gameObject.layer;

            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            blockerRect.SetParent(rootCanvas.transform, false);
            blockerRect.anchorMin = Vector3.zero;
            blockerRect.anchorMax = Vector3.one;
            blockerRect.sizeDelta = Vector2.zero;

            Canvas blockerCanvas = blocker.AddComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            Canvas dropdownCanvas = _dropdownList.GetComponent<Canvas>();
            blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
            blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

            Canvas parentCanvas = null;
            Transform parentTransform = m_Template.parent;
            while (parentTransform != null)
            {
                parentCanvas = parentTransform.GetComponent<Canvas>();
                if (parentCanvas != null)
                    break;

                parentTransform = parentTransform.parent;
            }

            if (parentCanvas != null)
            {
                Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
                for (int i = 0; i < components.Length; i++)
                {
                    Type raycasterType = components[i].GetType();
                    if (blocker.GetComponent(raycasterType) == null)
                    {
                        blocker.AddComponent(raycasterType);
                    }
                }
            }
            else
            {
                GetOrAddComponent<GraphicRaycaster>(blocker);
            }

            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = Color.clear;

            Button blockerButton = blocker.AddComponent<Button>();
            blockerButton.onClick.AddListener(Hide);

            CanvasGroup blockerCanvasGroup = blocker.AddComponent<CanvasGroup>();
            blockerCanvasGroup.ignoreParentGroups = true;

            return blocker;
        }
        private void DestroyBlocker(GameObject blocker)
        {
            Destroy(blocker);
        }
        private T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();
            return comp;
        }

        private void Add(List<int> indexs, bool sendCallback)
        {
            if (Application.isPlaying && (indexs == null || indexs.Count == 0 || m_Options.Count == 0))
                return;

            m_Values.AddRange(indexs);
            StandardizationValues(m_Values);
            RefreshShownValue();

            if (sendCallback)
            {
                m_OnValueChanged.Invoke(m_Values);
            }
        }
        private void Add(int index, bool sendCallback)
        {
            if (Application.isPlaying && (m_Values.Contains(index) || m_Options.Count == 0))
                return;

            m_Values.Add(index);
            StandardizationValues(m_Values);
            RefreshShownValue();

            if (sendCallback)
            {
                m_OnValueChanged.Invoke(m_Values);
            }
        }
        private void Remove(List<int> indexs, bool sendCallback)
        {
            if (Application.isPlaying && (indexs == null || indexs.Count == 0 || m_Options.Count == 0))
                return;

            for (int i = 0; i < indexs.Count; i++)
            {
                m_Values.Remove(indexs[i]);
            }
            StandardizationValues(m_Values);
            RefreshShownValue();

            if (sendCallback)
            {
                m_OnValueChanged.Invoke(m_Values);
            }
        }
        private void Remove(int index, bool sendCallback)
        {
            if (Application.isPlaying && (!m_Values.Contains(index) || m_Options.Count == 0))
                return;

            m_Values.Remove(index);
            StandardizationValues(m_Values);
            RefreshShownValue();

            if (sendCallback)
            {
                m_OnValueChanged.Invoke(m_Values);
            }
        }
        private void Clear(bool sendCallback)
        {
            m_Values.Clear();
            StandardizationValues(m_Values);
            RefreshShownValue();

            if (sendCallback)
            {
                m_OnValueChanged.Invoke(m_Values);
            }
        }
        private void StandardizationValues(List<int> indexs)
        {
            if (indexs.Count <= 0 || m_Options.Count <= 0)
                return;

            _duplicatesHashSet.Clear();
            _duplicatesList.Clear();
            _duplicatesList.AddRange(indexs);

            indexs.Clear();
            for (int i = 0; i < _duplicatesList.Count; i++)
            {
                int index = Mathf.Clamp(_duplicatesList[i], 0, m_Options.Count - 1);
                if (_duplicatesHashSet.Add(index))
                {
                    indexs.Add(index);
                }
            }
        }
    }
}