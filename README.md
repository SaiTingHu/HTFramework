# Unity HTFramework

一个开源的适用于Unity的简易、轻量级框架，更适用于应用软件、小型游戏项目。

## 环境

- Unity版本：2018.3.0及以上。

- .NET API版本：4.x。

## 模块简介

- [AspectTrack](https://wanderer.blog.csdn.net/article/details/85617377) - 根据AOP思想架构的一个面向切面的程序代码追踪模块，它可以跟踪每一个方法的调用，在调用前阻断该方法，亦或是更改其传入的实参，更改其返回值等！可以用于调用日志打印，系统运行监控等需求。

- [AssetBundleEditor](https://wanderer.blog.csdn.net/article/details/90081589) - 项目资源监控与资源打包统一编辑器，也可以对项目中的冗余资源进行管理和筛除。

- [Audio](https://wanderer.blog.csdn.net/article/details/89874351) - 项目中所有音频的管理器，提供统一的接口用来播放、暂停、停止各种音频资源。

- [Controller](https://wanderer.blog.csdn.net/article/details/89416110) - 封装了主角控制、主摄像机控制等，简化了大量重复代码。

- [Coroutiner](https://wanderer.blog.csdn.net/article/details/91492838) - 协程调度器，通过协程调度器启动的协程，将会处于调度器的监控中，可以通过CoroutinerTracker追踪面板查看协程的运行状态、运行耗时，可重启、终止任意协程。

- [DataSet](https://wanderer.blog.csdn.net/article/details/89395574) - 自定义数据集，任何需要存储数据的地方都可以定义为自定义数据集，包括预制的配置文件、运行时生成的动态数据、从后台拉取的各种数据等。

- [Event](https://wanderer.blog.csdn.net/article/details/85689865) - 可以将一切操作定义为具体的全局事件，通过订阅事件、抛出事件以驱动整个业务逻辑。

- ExceptionHandler - 异常处理者，当程序任何部位发生未知、未捕获的异常时，他会在这里被截获，并写入日志文件，同时支持在异常发生时打开指定程序（仅在PC平台），或者在异常发生时回馈日志到指定邮箱。

- [FSM](https://wanderer.blog.csdn.net/article/details/86073351) - 模拟一切可以抽象为有限状态机结构的业务逻辑，类似于角色动画、怪物AI、任意有独立逻辑的个体等。

- [Hotfix](https://wanderer.blog.csdn.net/article/details/90479971) - 以C#反射实现的轻量级热更新框架，开发非常方便，新项目只需要拉取框架源码后，一键即可创建热更新环境，之后便可以正常开发。

- [ILHotfix](https://github.com/SaiTingHu/HTFrameworkILHotfix)【可选模块】 - 基于ILRuntime实现的跨平台热更新框架，开发非常方便，新项目只需要拉取框架源码及本模块，一键即可创建热更新环境，之后便可以正常开发。

- [Input](https://wanderer.blog.csdn.net/article/details/89001848) - 将任意输入都定义为虚拟输入，再由Input模块统一调用，将是跨平台输入的最优解决方案。

- Main - 框架主模块，提供访问其他模块的快捷接口。

- Network - 网络客户端模块，主要支持TCP/UDP等协议。

- [ObjectPool](https://wanderer.blog.csdn.net/article/details/86610600) - 专用于GameObject类型的对象池，可以复用任意GameObject对象，以达到减少系统在频繁创建和删除对象时的开销。

- [Procedure](https://wanderer.blog.csdn.net/article/details/86998412) - 流程是框架的核心模块，也是最基本的模块，他贯穿整个框架的始终，从框架的生命周期开始，到生命周期结束，都会在流程间完成，同时，他又是一个强化版的有限状态机，当在多个流程间切换直至最终流程时，便代表整个系统的结束。

- [ReferencePool](https://wanderer.blog.csdn.net/article/details/87191712) - 可用于任意引用类型（除GameObject）的对象池，可以复用任意引用类型对象，以达到减少系统在频繁创建和删除对象时的开销。

- [Resource](https://wanderer.blog.csdn.net/article/details/88852698) - 资源加载管理器，主要用于动态加载资源（只支持异步加载），在加载中或加载完成后都可以进行自定义操作，现在主要支持Resource直接加载和AssetBundle加载，比如，UI模块就会自动调用资源管理器加载UI实体。

- [StepEditor](https://wanderer.blog.csdn.net/article/details/87712995) - 严格来说，StepEditor只是框架的一个内置工具，他最开始的用途是用来解决一系列冗长的线性任务，为了实现可视化和降低后期改动的复杂度，当然，也可以用作流程控制器。

- [UI](https://wanderer.blog.csdn.net/article/details/88125982) - 用于管理全局的UI实体，以省去手动创建UI实例、销毁UI实例等一系列操作，他可以在非常方便且省去不必要的开销优势下，让你条例清晰的组织和管控好任何复杂的UI结构。

- Utility - 框架实用工具，包括一些批处理工具及编辑器工具。

- [WebRequest](https://wanderer.blog.csdn.net/article/details/89886124) - 网络请求模块，主要用于与web服务器通信，比如请求某一网络链接或服务器接口，获得回复或下载网络上的资源。

## 使用方法

- 1.拉取框架到项目中的Assets文件夹下（Assets/HTFramework/）。

- 2.将框架根目录下的HTFramework.prefab拖到主场景，并删除主场景其他的东西（除了框架的主要模块，其他任何东西都应该是动态加载的）。

- 3.开始开发。
