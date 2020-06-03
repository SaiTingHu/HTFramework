![image](Editor/Main/Texture/HTFrameworkLOGOTitle.png)

[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/SaiTingHu/HTFramework/blob/master/LICENSE)
![version](https://img.shields.io/badge/version-Unstable-green.svg)
[![supported](https://img.shields.io/badge/supported-Unity-success.svg)](https://unity.com/)

# Unity HTFramework

一个开源的基于Unity的简易、轻量级、模块式框架，更适用于应用软件、小型游戏项目，可插拨特性使其能够最大兼容性的与其他框架、环境混合开发。

## 参考

- [EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework)。

## 环境

- Unity版本：2018.3.0及以上。

- .NET API版本：4.x。

## 编码规范

- 编码及代码审查遵循此规范[HTCODINGSTANDARD](https://github.com/SaiTingHu/HTFramework/blob/master/HTCODINGSTANDARD.md)。

## 模块简介

- [AspectTrack](https://wanderer.blog.csdn.net/article/details/85617377) - 根据AOP思想架构的一个面向切面的程序代码追踪模块，它可以跟踪每一个方法的调用，在调用前阻断该方法，亦或是更改其传入的实参，更改其返回值等！可以用于调用日志打印，系统运行监控等需求。

- [Audio](https://wanderer.blog.csdn.net/article/details/89874351) - 项目中所有音频的管理器，提供统一的接口用来播放、暂停、停止各种音频资源。

- [Controller](https://wanderer.blog.csdn.net/article/details/89416110) - 封装了主角控制、主摄像机控制等，简化了大量重复代码。

- [Coroutiner](https://wanderer.blog.csdn.net/article/details/91492838) - 协程调度器，通过协程调度器启动的协程，将会处于调度器的监控中，可以通过CoroutinerTracker追踪面板查看协程的运行状态、运行耗时，可重启、终止任意协程。

- [CustomModule](https://wanderer.blog.csdn.net/article/details/103390089) - 框架内置了多个常用的模块，如果想要添加自己的模块，通过CustomModule添加自定义模块即可，自定义模块拥有与内置模块完全一样的生命周期。

- [DataSet](https://wanderer.blog.csdn.net/article/details/89395574) - 自定义数据集，任何需要存储数据的地方都可以定义为自定义数据集，包括预制的配置文件、运行时生成的动态数据、从后台拉取的各种数据等。

- [Debug](https://wanderer.blog.csdn.net/article/details/102570194) - Debug模块自带Debugger运行时调试器，可以监控一些常规数据与软件运行环境，或是进行一些高级的操作，比如检索场景所有游戏对象（等效于编辑器内Hierarchy窗口的功能），检索游戏对象的所有组件（等效于编辑器内Inspector窗口的功能）。

- ECS - 实体-组件-系统，此ECS非Unity的ECS，并不一定会带来性能的提升，只是基于ECS的思想，建立在Unity现有的组件模式之上，以ECS模式进行开发可以避开项目后期繁重的继承链，提升开发速度和质量、以及项目稳定性。

- [Entity](https://wanderer.blog.csdn.net/article/details/101541066) - 实体管理器，除去UI以外，场景中的其余可见物体都应该抽象为Entity，在Entity之上配合FSM一起管理逻辑，将是一个不错的搭配。

- [Event](https://wanderer.blog.csdn.net/article/details/85689865) - 可以将一切操作定义为具体的全局事件，通过订阅事件、抛出事件以驱动整个业务逻辑。

- [ExceptionHandler](https://wanderer.blog.csdn.net/article/details/102894933) - 异常处理者，当程序任何部位发生未知、未捕获的异常时，他会在这里被截获，并写入日志文件，同时支持在异常发生时打开指定程序（仅在PC平台），或者在异常发生时回馈日志到指定邮箱。

- [FSM](https://wanderer.blog.csdn.net/article/details/86073351) - 模拟一切可以抽象为有限状态机结构的业务逻辑，类似于角色动画、怪物AI、任意有独立逻辑的个体等。

- [Hotfix](https://wanderer.blog.csdn.net/article/details/90479971) - 以C#反射实现的轻量级热更新框架，开发非常方便，新项目只需要拉取框架源码后，一键即可创建热更新环境，之后便可以用C#正常开发，目前已支持在热更新库中动态修复外界的任何方法，无需重新发布项目。

- [Input](https://wanderer.blog.csdn.net/article/details/89001848) - 将任意输入都定义为虚拟输入，再由Input模块统一调用，将是跨平台输入的最优解决方案。

- [Main](https://wanderer.blog.csdn.net/article/details/102956756) - 框架主模块，提供访问其他模块的快捷接口，还支持快捷设置脚本定义、指定全局主要数据类、设置项目授权、以及配置全局参数等。

- [Network](https://wanderer.blog.csdn.net/article/details/103575999) - 网络客户端模块，以单个通信管道为单位，每个管道均支持TCP/UDP等协议，可以为每个管道定义通信消息格式，基本能胜任一些常见的通信环境。

- [ObjectPool](https://wanderer.blog.csdn.net/article/details/86610600) - 专用于GameObject类型的对象池，可以复用任意GameObject对象，以达到减少系统在频繁创建和删除对象时的开销。

- [Procedure](https://wanderer.blog.csdn.net/article/details/86998412) - 流程是框架的核心模块，也是最基本的模块，他贯穿整个框架的始终，从框架的生命周期开始，到生命周期结束，都会在流程间完成，同时，他又是一个强化版的有限状态机，当在多个流程间切换直至最终流程时，便代表整个系统的结束。

- [ReferencePool](https://wanderer.blog.csdn.net/article/details/87191712) - 可用于任意引用类型（除GameObject）的对象池，可以复用任意引用类型对象，以达到减少系统在频繁创建和删除对象时的开销。

- [Resource](https://wanderer.blog.csdn.net/article/details/88852698) - 资源加载管理器，主要用于动态加载资源（只支持异步加载），在加载中或加载完成后都可以进行自定义操作，现在主要支持Resource直接加载和AssetBundle加载，比如，UI模块就会自动调用资源管理器加载UI实体。

- [StepEditor](https://wanderer.blog.csdn.net/article/details/87712995) - 步骤编辑器，严格来说，StepEditor只是框架的一个内置工具，他最开始的用途是用来解决一系列冗长的线性任务，为了实现可视化和降低后期改动的复杂度，当然，也可以用作流程控制器。

- [TaskEditor](https://wanderer.blog.csdn.net/article/details/104317219) - 任务编辑器，可以自定义任务点，设置任务达成条件，多个任务点组成一个任务内容，使用一系列任务内容完成角色扮演的设计。

- [UI](https://wanderer.blog.csdn.net/article/details/88125982) - 用于管理全局的UI实体，以省去手动创建UI实例、销毁UI实例等一系列操作，他可以在非常方便且省去不必要的开销优势下，让你条例清晰的组织和管控好任何复杂的UI结构。

- [Utility](https://wanderer.blog.csdn.net/article/details/102971712) - 框架实用工具，包括一些批处理工具及编辑器工具。

- [WebRequest](https://wanderer.blog.csdn.net/article/details/89886124) - 网络请求模块，主要用于与web服务器通信，比如请求某一网络链接或服务器接口，获得回复或下载网络上的资源。

- [AI](https://github.com/SaiTingHu/HTFrameworkAI)【可选模块】 - AI相关模块，比如A*寻路以及各种人工智能模块。

- [ILHotfix](https://github.com/SaiTingHu/HTFrameworkILHotfix)【可选模块】 - 基于ILRuntime实现的跨平台热更新框架，开发非常方便，新项目只需要拉取框架源码及本模块，一键即可创建热更新环境，之后便可以正常开发。

- [XLua](https://github.com/SaiTingHu/HTFrameworkXLua)【可选模块】 - 本模块旨在结合XLua与框架的资源加载策略，快速实现热更流程，并优化了开发环境，使得开发人员可以最低成本的投入到Lua业务开发。

- [GameComponent](https://github.com/SaiTingHu/HTFrameworkGameComponent)【可选模块】 - 游戏组件模块，本模块整合了多个游戏开发过程中可能会涉及到的子系统或组件，包括但不仅限于文件系统、本地化系统、新手引导系统、时间系统等，且各个系统或组件均为可插拨式，可一键移除不需要的系统或组件，或再次一键导入。

## 内置工具

- [Unity Asset Bundle Browser tool](https://docs.unity3d.com/Manual/AssetBundles-Browser.html)。

- [Dotween Free](http://dotween.demigiant.com/)。

## 注意事项

- 框架中所有On开头的函数为生命周期函数或回调函数，均由框架呼叫，请勿手动调用。

- 框架中的Procedure（流程）必须包含至少一个流程才能正确构建项目，而其他模块，如无需要，均可以不使用。

## 演示代码

- [入门级Demo](https://github.com/SaiTingHu/HTFrameworkDemo)。

- 应用级Demo。

## 使用方法

- 1.拉取框架到项目中的Assets文件夹下（Assets/HTFramework/），或以添加子模块的形式。

- 2.在入口场景的层级（Hierarchy）视图点击右键，选择 HTFramework -> Main Environment（创建框架主环境），并删除入口场景其他的东西（除了框架的主要模块，其他任何东西都应该是动态加载的）。

- 3.参阅各个模块的帮助文档，开始开发。
