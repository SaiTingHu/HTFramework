using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #SetParent 指令
    /// </summary>
    internal sealed class SentenceSetParent : Sentence
    {
        public string ParentPath;

        private GameObject _parent;

        /// <summary>
        /// 目标
        /// </summary>
        public GameObject Parent
        {
            get
            {
                if (_parent == null && !string.IsNullOrEmpty(ParentPath))
                {
                    int index = ParentPath.IndexOf('/');
                    if (index > 0)
                    {
                        string parent = ParentPath.Substring(0, index);
                        string child = ParentPath.Substring(index + 1);
                        _parent = GameObject.Find(parent).FindChildren(child);
                    }
                    else
                    {
                        _parent = GameObject.Find(ParentPath);
                    }
                }
                return _parent;
            }
        }

        public override void Execute()
        {
            if (Target && Parent)
            {
                Target.transform.SetParent(Parent.transform);
            }
        }
        public override void Reset()
        {
            base.Reset();

            ParentPath = null;
            _parent = null;
        }
    }
}