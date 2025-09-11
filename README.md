![image](Editor/Main/Texture/HTFrameworkLOGOTitle.png)

[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/SaiTingHu/HTFramework/blob/master/LICENSE)
[![version](https://img.shields.io/github/v/release/SaiTingHu/HTFramework?color=green&label=version)](https://github.com/SaiTingHu/HTFramework/releases)
[![supported](https://img.shields.io/badge/supported-Unity-success.svg)](https://unity.com/)

# Unity HTFramework

HTFramework是基于Unity的一个快速开发框架，集需求模块化、代码重用性、实用便捷性、功能高内聚、统一编码规范、可扩展性、可维护性、可通用性、可插拨性为一体，并提供持续维护、升级。

## 环境

- Unity版本：2022.3.34。

- .NET API版本：.NET Framework。

## 编码规范

- 编码及代码审查遵循此规范[HTCODINGSTANDARD](https://github.com/SaiTingHu/HTFramework/blob/master/HTCODINGSTANDARD.md)。

## 文档与博客

- Blog: [wanderer.blog.csdn.net](https://wanderer.blog.csdn.net/category_9283445.html)。

## 游戏实战教程

- ![图标](Editor/Main/Texture/ReadMeIcon/TankWar.png) [游戏实战教程 - 超级坦克大战](https://wanderer.blog.csdn.net/category_10133279.html)。

- ![图标](Editor/Main/Texture/ReadMeIcon/FTG2D.png) [游戏实战教程 - FTG2D横版自由格斗](https://wanderer.blog.csdn.net/category_10732753.html)。

- ![图标](Editor/Main/Texture/ReadMeIcon/RPG2D.png) [游戏实战教程 - RPG2D角色扮演与回合制战棋](https://wanderer.blog.csdn.net/category_12698179.html)。

- ![图标](Editor/Main/Texture/ReadMeIcon/MMORPG.png) 游戏实战教程 - MMORPG大型多人在线角色扮演 + 中国象棋联网对战（敬请期待）。

## 模块简介

- [AspectTrack](https://wanderer.blog.csdn.net/article/details/85617377) - 根据AOP思想设计的一个面向切面的程序代码追踪模块，它可以跟踪每一个方法的调用，在调用前阻断该方法，亦或是更改其传入的实参，更改其返回值等！可以用于调用日志打印，系统运行监控等需求。

- [Audio](https://wanderer.blog.csdn.net/article/details/89874351) - 项目中所有音频的管理器，提供统一的接口用来播放、暂停、停止各种音频资源。

- [Controller](https://wanderer.blog.csdn.net/article/details/89416110) - 封装了主角控制、主摄像机控制、射线检测等，比如支持自由操作的自由控制视角，可扩展的第一人称、第三人称视角。

- [Coroutiner](https://wanderer.blog.csdn.net/article/details/91492838) - 协程调度器，通过协程调度器启动的协程，将会处于调度器的监控中，可以通过CoroutinerTracker追踪面板查看协程的运行状态、运行耗时，可重启、终止任意协程。

- [CustomModule](https://wanderer.blog.csdn.net/article/details/103390089) - 框架内置了多个常用的模块，如果想要添加自己的模块，通过CustomModule添加自定义模块即可，自定义模块拥有与内置模块完全一样的生命周期。

- [DataSet](https://wanderer.blog.csdn.net/article/details/89395574) - 数据集管理器，任何需要存储数据的地方都可以定义为自定义数据集，包括预制的配置文件、运行时生成的动态数据、从后台拉取的各种数据等。

- [Debug](https://wanderer.blog.csdn.net/article/details/102570194) - Debug模块自带Debugger运行时调试器，可以监控一些常规数据与软件运行环境，或是进行一些高级的操作，比如检索场景所有游戏对象（等效于编辑器内Hierarchy窗口的功能），检索游戏对象的所有组件（等效于编辑器内Inspector窗口的功能）。

- [ECS](https://wanderer.blog.csdn.net/article/details/106619485) - 实体-组件-系统，此ECS非Unity的ECS，并不一定会带来性能的提升，只是基于ECS的思想，建立在Unity现有的组件模式之上，以ECS模式进行开发可以避开项目后期繁重的继承链，提升开发速度和质量、以及项目稳定性。

- [Entity](https://wanderer.blog.csdn.net/article/details/101541066) - 实体管理器，除去UI以外，场景中的其余可见物体都应该抽象为Entity，在Entity之上配合FSM一起管理逻辑，将是一个不错的搭配。

- [Event](https://wanderer.blog.csdn.net/article/details/85689865) - 可以将一切操作定义为具体的全局事件，通过订阅事件、抛出事件以驱动整个业务逻辑。

- [Exception](https://wanderer.blog.csdn.net/article/details/102894933) - 异常处理器，当程序任何部位发生未知、未捕获的异常时，他会在这里被截获，并写入日志文件，同时支持在异常发生时打开指定程序（仅在PC平台），或者在异常发生时回馈日志到指定邮箱。

- [FSM](https://wanderer.blog.csdn.net/article/details/86073351) - FSM模拟一切可以抽象为有限状态机结构的业务逻辑，类似于角色动画、怪物AI、任意有独立逻辑的个体等。

- [Hotfix](https://wanderer.blog.csdn.net/article/details/90479971) - 以C#反射实现的轻量级热更新框架，开发非常方便，新项目只需要拉取框架源码后，一键即可创建热更新环境，之后便可以用C#正常开发，目前已支持在热更新库中动态修复外界的任何方法，无需重新发布项目。

- [Input](https://wanderer.blog.csdn.net/article/details/89001848) - 将任意输入都定义为虚拟输入，再由Input模块统一调用，将是跨平台输入的最优解决方案。

- [Instruction](https://wanderer.blog.csdn.net/article/details/130918484) - 指令系统为Unity动态修补程序、热更新等提供了另一种补充方案，我们可以将任意一段指令代码即时编译并执行（请放心，即时编译的性能开销极低），达到运行时随意修改程序功能的需求。

- [Main](https://wanderer.blog.csdn.net/article/details/102956756) - 框架主模块，提供访问其他模块的快捷接口，还支持快捷设置脚本定义、指定全局主要数据类、设置项目授权、以及配置全局参数等。

- [Network](https://wanderer.blog.csdn.net/article/details/103575999) - 网络客户端模块，以单个通信管道为单位，每个管道均支持TCP/UDP等协议，可以为每个管道定义通信消息格式，基本能胜任一些常见的通信环境。

- [ObjectPool](https://wanderer.blog.csdn.net/article/details/86610600) - 专用于GameObject类型的对象池，可以复用任意GameObject对象，以达到减少系统在频繁创建和删除对象时的开销。

- [Procedure](https://wanderer.blog.csdn.net/article/details/86998412) - 流程是框架的核心模块，也是最基本的模块，他贯穿整个框架的始终，从框架的生命周期开始，到生命周期结束，都会在流程间完成，同时，他又是一个强化版的有限状态机，当在多个流程间切换直至最终流程时，便代表整个系统的结束。

- [ReferencePool](https://wanderer.blog.csdn.net/article/details/87191712) - 可用于任意引用类型（除GameObject）的对象池，可以复用任意引用类型对象，以达到减少系统在频繁创建和删除对象时的开销。

- [Resource](https://wanderer.blog.csdn.net/article/details/88852698) - 资源管理器，主要用于动态加载资源（只支持异步加载），在加载中或加载完成后都可以进行自定义操作，现在主要支持Resource加载模式、AssetBundle加载模式、Addressables加载模式，比如，UI模块就会自动调用资源管理器加载UI实体。

- [StepMaster](https://wanderer.blog.csdn.net/article/details/87712995) - 步骤编辑器，用来解决一系列冗长的序列任务流程，支持可视化编辑和自定义步骤助手。

- [TaskMaster](https://wanderer.blog.csdn.net/article/details/104317219) - 任务编辑器，可以自定义任务点，设置任务达成条件，多个任务点组成一个任务内容，使用一系列任务内容完成角色扮演的设计。

- [UI](https://wanderer.blog.csdn.net/article/details/88125982) - 用于管理全局的UI实体，以省去手动创建UI实例、销毁UI实例等一系列操作，他可以在非常方便且省去不必要的开销优势下，让你条例清晰的组织和管控好任何复杂的UI结构。

- [Utility](https://wanderer.blog.csdn.net/article/details/102971712) - 实用工具，包含大量编辑器实用工具及运行时实用工具。

- [WebRequest](https://wanderer.blog.csdn.net/article/details/89886124) - 网络请求模块，主要用于与web服务器通信，比如请求某一网络链接或服务器接口，获得回复或下载网络上的资源。

- [AI](https://github.com/SaiTingHu/HTFrameworkAI)【可选模块】 - AI相关模块，比如A*寻路以及各种人工智能模块。

- [Deployment](https://github.com/SaiTingHu/HTFrameworkDeployment)【可选模块】 - 轻量级资源部署管线，整合资源打包、资源版本构建、资源版本更新为一体，快速实现资源部署和交付游戏。

- [GameComponent](https://github.com/SaiTingHu/HTFrameworkGameComponent)【可选模块】 - 游戏组件模块，本模块整合了多个游戏开发过程中可能会涉及到的子系统或组件，包括但不仅限于文件系统、本地化系统、新手引导系统、时间系统、AVG2D系统、FTG2D系统、RPG2D系统等，且各个系统或组件均为可插拨式，可一键移除不需要的系统或组件，或再次一键导入。

## 内置工具

- [AssetBundle Browser 1.7.0](https://docs.unity3d.com/Manual/AssetBundles-Browser.html)。

- [Dotween Free 1.2.632](http://dotween.demigiant.com/)。

- [LitJson 0.17.0](https://github.com/LitJSON/litjson)。

## 演示代码

- [入门级Demo](https://github.com/SaiTingHu/HTFrameworkDemo)。

## 注意事项

- 1.框架中所有On开头的函数为生命周期函数或回调函数，均由框架呼叫，请勿手动调用。

- 2.框架中的Procedure（流程）必须包含至少一个流程才能正确构建项目，而其他模块，如无需要，均可以不使用。

## 主要特性

- 1.入口场景为一切的开始，也为一切的结束，整个游戏的生命周期都将在入口场景中完成，不建议使用多场景切换模式，如果确实需要使用，可以将其他场景打入AB包内加载。

- 2.入口场景中只包含框架主模块（或其他自定义主模块、主控制器），其他任何东西都应该是动态加载的。

## 其他特性

- [Addressables](https://wanderer.blog.csdn.net/article/details/140700085) - 使用Addressables可寻址系统。

- [Assets Master](https://wanderer.blog.csdn.net/article/details/107974865) - 资产管理器，允许你在编辑模式或是运行模式非常直观的查看和管理当前打开场景中的你感兴趣的资产。

- [Auto Attach Namespace](https://wanderer.blog.csdn.net/article/details/139263711) - 新建脚本时，自动向脚本添加命名空间。

- [Code Snippet Executer](https://wanderer.blog.csdn.net/article/details/139546629) - 适用于代码片段测试、单元测试的编辑器工具。

- [HybridCLR](https://wanderer.blog.csdn.net/article/details/140084724) - 使用HybridCLR热更新。

- [Inspector](https://wanderer.blog.csdn.net/article/details/108117002) - 支持通过简单的在序列化字段上添加特性标记从而实现在Inspector界面自定义多种实用的检视器效果。

- [License](https://wanderer.blog.csdn.net/article/details/104614122) - 授权验证功能支持在你的游戏启动时，执行指定的授权检查，可以是本地授权检查，也可以是远程授权检查，只有授权检查通过才能正常进入游戏，如果授权检查失败，框架将会瘫痪整个程序，无论用户做出任何补救措施都将是徒劳。

- [Location](https://wanderer.blog.csdn.net/article/details/138852187) - 使用Location设置Transform的位置、旋转、缩放。

- [Markdown Text](https://wanderer.blog.csdn.net/article/details/142280249) - MarkdownText为Text的扩展加强版，支持在运行时解析并显示Markdown格式的文本。

- [MVVM](https://wanderer.blog.csdn.net/article/details/109245614) - UI的数据驱动模式。

- [Project Folder Locker](https://wanderer.blog.csdn.net/article/details/146094956) - 在Project窗口中将插件文件夹、或不需要经常访问的文件夹加锁，保持Project窗口的干净整洁。

- [Scene Handler](https://wanderer.blog.csdn.net/article/details/108281901) - 支持通过简单的在序列化字段上添加特性标记从而实现在Scene界面自定义多种实用的控制柄。

- [Settings](https://wanderer.blog.csdn.net/article/details/104610857) - 全局设置面板可以快捷、统一的设置全局的一些参数，还可以自定义设置项，用来设置自己的任何游戏参数。

- [Search By Layer Or Tag](https://wanderer.blog.csdn.net/article/details/144815624) - 通过Tag、Layer批量搜索物体。

- [StandardizingNaming](https://wanderer.blog.csdn.net/article/details/124470077) - 建立统一的标准化命名规范。

- [SerializableDictionary](https://wanderer.blog.csdn.net//article/details/146536539) - 可序列化字典、可序列化哈希集，用于替代不可序列化的Dictionary、HashSet类型。

- [SaveDataRuntime](https://wanderer.blog.csdn.net/article/details/146909158) - 运行时保存组件参数、预制体。

- [ScrollList](https://wanderer.blog.csdn.net/article/details/148016087) - 滚动数据列表，更方便的进行数据的增、删、改、显示。

- [Table View](https://wanderer.blog.csdn.net/article/details/120796924) - 使用TableView可以很方便的在编辑器中绘制表格视图。

- [UDateTime](https://wanderer.blog.csdn.net/article/details/149958396) - 可序列化日期时间（附运行时、编辑器日期拾取器）。

- [控制反转](https://wanderer.blog.csdn.net/article/details/122300055) - 在Unity中使用控制反转模式。

## 使用方法

- 1.拉取框架到项目中的Assets文件夹下（Assets/HTFramework/），或以添加子模块的形式。

- 2.在入口场景的层级（Hierarchy）视图点击右键，选择 HTFramework -> Main Environment（创建框架主环境），并删除入口场景其他的东西（除了框架的主要模块，其他任何东西都应该是动态加载的）。

- 3.参阅各个模块的帮助文档，开始开发。
