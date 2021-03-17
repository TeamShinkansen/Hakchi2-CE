# hakchi2 CE

这是 princess_daphie，DanTheMan827 和 skogaby 编写的 hakchi2（由ClusterM编写）的分支。 
此分支的目的不仅在于提供新的 UI 功能和增强功能，而且还将 hakchi2 的核心与其他模块生态系统（即 USB 主机支持，SD 支持）保持最新。

此应用程序可以向您的 NES/SNES Classic Mini 或 Famicom Mini 添加更多游戏（游戏ROM）。
您只需要通过 Micro-USB 线将其连接到 Windows PC。无需焊接或拆卸。

https://github.com/TeamShinkansen/hakchi2

### 功能
* 更改任何游戏设置（包括命令行参数）
* 使用包含的数据库自动填充所有游戏数据
* 自动检查支持的游戏
* 使用谷歌图片搜索游戏封面
* 对于 NES 游戏，使用 [Game Genie](https://en.wikipedia.org/wiki/Game_Genie) ；包括 Game Genie 数据库
* 自动修补问题游戏（包括许多热门游戏的补丁）
* 一次上传数百个游戏
* 使用组合按钮（而不是“重置”按钮）返回到主菜单
* 启用 A/B 自动连发
* 模拟第二个控制器上的开始按钮（对于 Famicom Mini）
* 禁用 seizure  保护
* 允许安装用户模块以添加更多功能（甚至支持 SNES/N64/Genesis/ 等，音乐替换，主题等）
* 允许用户扩展其系统的存储（提供 USB OTG 集线器或 SD 分组模块）

## 因此，您是第一个破解 NES Classic Mini 的人吗？
没有！ 是俄罗斯人 madmonkey，他首先发布了 NES Classic Mini 的破解。他创建了原始的 “hakchi” 工具。
但是，它不是很友好的，所以我决定创建一个易于所有人使用的工具-不仅限于 Linux 用户。 
我将其命名为“ hakchi2”，因为我不喜欢取名字。所以我的第一个版本是2.0版本 :)

## 我怎样使用这个工具？
基本上你只需要把它放在硬盘上的某个地方（不需要安装），运行它，按“添加更多游戏”，选择一些游戏ROM并按“同步”。应用程序将指导您完成此过程。

## 这个工具实际上是如何工作的？
您无需担心。但是，如果您真的想知道，它使用FEL模式。 FEL是Allwinner设备上的BootROM中包含的低级子例程。它用于使用USB进行设备的初始编程和恢复。因此，我们可以将一些代码上传到RAM中并执行它。通过这种方式，我们可以读取Linux内核（是的NES Classic Mini和Famicom Mini运行Linux操作系统），编写内核或从内存执行内核而无需将其写入闪存。因此，我们可以转储NES Mini的内核映像，解压缩它，添加一些游戏并运行一个脚本，该脚本会将它们复制回Flash，重新打包，上传并执行。但是，游戏目录位于只读分区上。因此，我们还需要使用特殊脚本创建并刷新自定义内核，该脚本将在可写分区上创建沙箱文件夹并将其安装在原始游戏文件夹上。这意味着您的原始文件是安全的：即使有需要，也无法以任何方式删除或损害原始文件。对于内核补丁，我的应用程序仅执行其他应用程序，这就是为什么有“ tools”文件夹的原因。

## 如果我还有其他问题怎么办？
常见问题解答中回答了更多常见问题：
https://github.com/TeamShinkansen/hakchi2/wiki/FAQ
