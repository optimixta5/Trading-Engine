using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TradingEngineServer.Orders.OrderStatuses;
using TradingEngineServer.Rejects;

namespace TradingEngineServer.Orderbook
{
    public class OrderbookResult
    {
        // METHODS //
        public void Merge(OrderbookResult obr)
        {
            AddNewOrderStatuses(obr.NewOrderStatuses);
            AddModifyOrderStatuses(obr.ModifyOrderStatuses);
            AddCancelOrderStatuses(obr.CancelOrderStatuses);
            AddRejections(obr.Rejections);
        }

        public void AddNewOrderStatuses(IReadOnlyList<NewOrderStatus> os)
        {
            _newOrderStatuses.AddRange(os);
        }

        public void AddCancelOrderStatuses(IReadOnlyList<CancelOrderStatus> cos)
        {
            _cancelOrderStatuses.AddRange(cos);
        }

        public void AddModifyOrderStatuses(IReadOnlyList<ModifyOrderStatus> mos)
        {
            _modifyOrderStatuses.AddRange(mos);
        }

        public void AddRejections(IReadOnlyList<Rejection> ros)
        {
            _rejections.AddRange(ros);
        }

        public void AddNewOrderStatus(NewOrderStatus os)
        {
            _newOrderStatuses.Add(os);
        }

        public void AddCancelOrderStatus(CancelOrderStatus cos)
        {
            _cancelOrderStatuses.Add(cos);
        }

        public void AddModifyOrderStatus(ModifyOrderStatus mos)
        {
            _modifyOrderStatuses.Add(mos);
        }

        public void AddRejection(Rejection r)
        {
            _rejections.Add(r);
        }

        // GETTERS //
        public IReadOnlyList<NewOrderStatus> NewOrderStatuses
        {
            get
            {
                return new ReadOnlyCollection<NewOrderStatus>(_newOrderStatuses);
            }
        }

        public IReadOnlyList<ModifyOrderStatus> ModifyOrderStatuses
        {
            get
            {
                return new ReadOnlyCollection<ModifyOrderStatus>(_modifyOrderStatuses);
            }
        }

        public IReadOnlyList<CancelOrderStatus> CancelOrderStatuses
        {
            get
            {
                return new ReadOnlyCollection<CancelOrderStatus>(_cancelOrderStatuses);
            }
        }

        public IReadOnlyList<Rejection> Rejections
        {
            get
            {
                return new ReadOnlyCollection<Rejection>(_rejections);
            }
        }


        // PROPERTIES //
        public bool HasNewOrderStatus 
        { 
            get
            {
                return _newOrderStatuses.Any();
            }
        }

        public bool HasCancelOrderStatus
        {
            get
            {
                return _cancelOrderStatuses.Any();
            }
        }

        public bool HasModifyOrderStatus
        {
            get
            {
                return _modifyOrderStatuses.Any();
            }
        }

        public bool HasRejection
        {
            get
            {
                return _rejections.Any();
            }
        }

        // PRIVATE // 
        private List<NewOrderStatus> _newOrderStatuses = new List<NewOrderStatus>();
        private List<CancelOrderStatus> _cancelOrderStatuses = new List<CancelOrderStatus>();
        private List<ModifyOrderStatus> _modifyOrderStatuses = new List<ModifyOrderStatus>();
        private List<Rejection> _rejections = new List<Rejection>();
    }
}
