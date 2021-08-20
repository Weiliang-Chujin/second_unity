namespace Core.Scripts.Dialogs.BasicCore
{
    public enum DialogShowOption
    {
        kStack, // 打开下一个之后如果当前有开着的会隐藏，新的关闭之后会重新打开
        kReplaceCurrent, // 暂时没用
        kDontShowIfOthersShowing, // 暂时没用
        kToast, // 直接覆盖，不做处理
    }
}