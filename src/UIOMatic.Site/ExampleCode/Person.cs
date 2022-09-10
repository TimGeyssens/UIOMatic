using System.ComponentModel.DataAnnotations;
using NPoco;
using UIOMatic.Attributes;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UIOMatic.Site.ExampleCode
{
    [UIOMatic("people", "People", "Person", FolderIcon = "icon-users", ItemIcon = "icon-user", RenderType =Enums.UIOMaticRenderType.List, ShowOnSummaryDashboard = true)]
    [TableName("People")]
    public class Person
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Required]
        [UIOMaticListViewField]
        [UIOMaticField(Name = "First name", Description = "Enter the persons first name")]
        public string FirstName { get; set; }

        [Required]
        [UIOMaticListViewField]
        [UIOMaticField(Name = "Last name", Description = "Enter the persons last name")]
        public string LastName { get; set; }

        [UIOMaticField(Name = "Picture", Description = "Select a picture", View = UIOMatic.Constants.FieldEditors.File)]
        public string Picture { get; set; }

        [UIOMaticField(Description = "Example checkbox property",
            View = UIOMatic.Constants.FieldEditors.CheckBox)]
        public bool CheckBox { get; set; }

        [UIOMaticField(Description = "Example content picker property",
            View = UIOMatic.Constants.FieldEditors.PickerContent)]
        public int ContentPicker { get; set; }

        [UIOMaticField(Description = "Example date property",
            View = UIOMatic.Constants.FieldEditors.Date)]
        public DateTime Date { get; set; }

        [UIOMaticField(Description = "Example datetime property",
            View = UIOMatic.Constants.FieldEditors.DateTime)]
        public DateTime DateTime { get; set; }

        [UIOMaticField(Description = "Example datetimeoffset property",
            View = UIOMatic.Constants.FieldEditors.DateTimeOffset)]
        public DateTimeOffset DateTimeOffset { get; set; }

        [UIOMaticField(Description = "Example datetimeutc property",
            View = UIOMatic.Constants.FieldEditors.DateTimeUtc)]
        public DateTime DateTimeUtc { get; set; }

        [UIOMaticField(Description =  "Example file property", View = UIOMatic.Constants.FieldEditors.File)]
        public string File { get; set; }

        [UIOMaticField(Description = "Example label property",
            View = UIOMatic.Constants.FieldEditors.Label)]
        public string Label { get; set; }

        [UIOMaticField(Description = "Example link property", View = UIOMatic.Constants.FieldEditors.Link, Config = "{'URL': 'https://umbraco.com'}")]
        public string Link { get; set; }

        [UIOMaticField(Description = "Example media picker property",
            View = UIOMatic.Constants.FieldEditors.PickerMedia)]
        public int MediaPicker { get; set; }

        [UIOMaticField(Description = "Example member picker property",
            View = UIOMatic.Constants.FieldEditors.PickerMember)]
        public int MemberPicker { get; set; }

        [UIOMaticField(Description = "Example number property",
            View = UIOMatic.Constants.FieldEditors.Number)]
        public int Number { get; set; }

        [UIOMaticField(Description = "Example password property",
            View = UIOMatic.Constants.FieldEditors.Password)]
        public string Password { get; set; }

        [UIOMaticField(Description = "Example textarea property",
            View = UIOMatic.Constants.FieldEditors.Textarea)]
        public string Textarea { get; set; }

        [UIOMaticField(Description = "Example textfield property",
            View = UIOMatic.Constants.FieldEditors.Textfield)]
        public string Textfield { get; set; }

        [UIOMaticField(Description = "Example user picker",
            View = UIOMatic.Constants.FieldEditors.PickerUser)]
        public string UserPicker { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

    }
}
