using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarWinInventory.Models
{
    public enum ScannerResult
    {
        UnknownSku,
        AddedToStock,
        Scanned,
        UnknownError
    }

    public class ScannerResultMessage
    {
        public ScannerResultMessage(string barcode, ScannerResult scannerResult)
        {
            this.ScannerResult = scannerResult;
            this.Barcode = barcode;

        }

        public string Barcode { get; set; }
        public ScannerResult ScannerResult { get; set; }

    }
}
