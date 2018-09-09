using System.Collections.Generic;

namespace Marketing.ViewModelComposition.Events
{
    public class AvailableProductsLoaded
    {
        public IDictionary<int, dynamic> AvailableProductsViewModel { get; set; }
    }
}
