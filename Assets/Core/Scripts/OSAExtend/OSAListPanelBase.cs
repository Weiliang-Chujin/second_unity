using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Core.Scripts.BasicModules.Components;

namespace Core.Scripts.OSAExtend
{
    public class OSAListPanelBase<TListAdapter, TData, TViewsHolder> : WrappedMonoBehaviour
        where TListAdapter : OSAListBase<TData, TViewsHolder>
        where TViewsHolder : BaseItemViewsHolder, new()
    {
        public TListAdapter listAdapter;

        private bool isInit;

        /// <summary>
        /// Init Info Without Show
        /// </summary>
        public virtual void Init()
        {
            if (!isInit)
            {
                InitSetup();
            }
        }

        /// <summary>
        /// used only for the first init call
        /// multiple calls are not allowed
        /// </summary>
        public virtual void InitSetup()
        {
            isInit = true;
            listAdapter.Setup();
        }

        /// <summary>
        /// Refresh OSA
        /// </summary>
        /// <param name="guildList">Guild Info List</param>
        protected virtual void RefreshList(List<TData> guildList)
        {
            listAdapter.SetItems(guildList);
        }

        /// <summary>
        /// OSA pull release callback
        /// </summary>
        /// <param name="sign">OSA View Normalized Pos</param>
        public virtual void OnPullReleased(float sign)
        {
            listAdapter.Parameters.SetDragEnable(false);

            if (sign > 0)
            {
                listAdapter.PullDown();

                if (AbleDoPullDownCallback())
                {
                    OnPullDownReleased();
                }
            }
            else if (sign < 0)
            {
                if (AbleDoPullUpCallback())
                {
                    OnPullUpRelease();

                    // call adapter
                    listAdapter.PullUp();
                }
                else
                {
                    listAdapter.Parameters.SetDragEnable(true);
                    listAdapter.FinishPullToRefresh();
                }
            }
        }

        /// <summary>
        /// Check whether the pull down callback is executable
        /// </summary>
        /// <returns>If executable return True</returns>
        protected virtual bool AbleDoPullDownCallback()
        {
            return false;
        }

        /// <summary>
        /// Check whether the pull up callback is executable
        /// </summary>
        /// <returns>If executable return True</returns>
        protected virtual bool AbleDoPullUpCallback()
        {
            return false;
        }

        /// <summary>
        /// pull down callback
        /// </summary>
        protected virtual void OnPullDownReleased()
        {
        }

        /// <summary>
        /// pull up callback
        /// </summary>
        protected virtual void OnPullUpRelease()
        {
        }
    }
}