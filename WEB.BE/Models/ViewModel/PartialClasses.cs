using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace WEB.BE.Models.ViewModel
{
    // Sử dụng [MetadataType] để liên kết Entity Category với Metadata Class
    [MetadataType(typeof(CategoryMetadata))]
    public class PartialClasses
    {
    }
}