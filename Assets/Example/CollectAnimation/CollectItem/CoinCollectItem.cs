namespace AnimationCollector
{
    public class CoinCollectItem : BaseCollectItem
    {
        protected override void Complete()
        {
            base.Complete();

//            AudioHelper.IntervalSound(AudioConstant.single_coin_collect);
        }
    }
}