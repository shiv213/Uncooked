using System.Collections.Generic;

namespace Uncooked {
    internal class OrderData {
        
        public readonly string orderName;
        public List<string> ingredientsDone;
        public ClientPlate plate;

        public OrderData(string orderName) {
            this.orderName = orderName;
        }

    }
}