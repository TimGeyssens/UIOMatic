//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using UIOMatic.Atributes;
//using UIOMatic.Enums;

//namespace Example.Models
//{
//    [UIOMatic("Test", "icon-users", "icon-user", RenderType = UIOMaticRenderType.List)]
//    public class Test
//    {
//        [UIOMaticField("Checkbox","Example checkbox property",
//            View= "checkbox")]
//        public bool CheckBox { get; set; }

//        [UIOMaticField("Date", "Example date property",
//            View = "date")]
//        public DateTime Date { get; set; }

//        [UIOMaticField("Datetime", "Example datetime property",
//            View = "datetime")]
//        public DateTime DateTime { get; set; }

//        [UIOMaticField("Dropdown", "Example dropdown property",
//            View = "dropdown")]
//        public string Dropdown { get; set; }

//        [UIOMaticField("File", "Example file property",
//            View = "file")]
//        public string File { get; set; }

//        [UIOMaticField("Label", "Example label property",
//            View = "label")]
//        public string Label { get; set; }

//        [UIOMaticField("Number", "Example number property",
//            View = "number")]
//        public int Number { get; set; }

//        [UIOMaticField("Password", "Example password property",
//            View = "password")]
//        public string Password { get; set; }

//        [UIOMaticField("Content picker", "Example content picker property",
//            View = "pickers.content")]
//        public int ContentPicker { get; set; }

//        [UIOMaticField("Media picker", "Example media picker property",
//            View = "pickers.media")]
//        public int  MediaPicker { get; set; }

//        [UIOMaticField("TextArea", "Example textarea property",
//            View = "checkbox")]
//        public string Textarea { get; set; }

//        [UIOMaticField("Textfield", "Example textfield property",
//            View = "textfield")]
//        public string Textfield { get; set; }
//    }
//}