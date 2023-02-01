
using System;
using FlexitHisMVC.Models;
namespace FlexitHisMVC.Models {
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}


