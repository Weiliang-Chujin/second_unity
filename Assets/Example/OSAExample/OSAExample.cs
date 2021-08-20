using System;
using Com.TheFallenGames.OSA.Core;

namespace Core.Scripts.OSAExtend.Example
{
    public class ExampleData
    {
        public int count;
        public string title;
    }

    public class OSAExample : OSAListBase<ExampleData, ExampleViewsHolder>
    {
        private Action onClick;
        protected override void SetViewsHolderData(ExampleViewsHolder viewsHolder, ExampleData model)
        {
            viewsHolder.cell.SetData(model);
            viewsHolder.cell.SetupClickAction(onClick);
        }
    }

    public class ExampleViewsHolder : BaseItemViewsHolder
    {
        public ExampleCell cell;

        public override void CollectViews()
        {
            base.CollectViews();
            cell = root.GetComponent<ExampleCell>();
        }
    }
}