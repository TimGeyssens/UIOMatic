# ![ui-o-matic logo](img/uiomatic.png) #

UI-O-Matic allows you to auto generate an integrated crud UI in Umbraco v7 and v8 for a db table based on a [petapoco](http://www.toptensoftware.com/petapoco/) / [npoco](https://github.com/schotime/NPoco) (the default ORM in Umbraco) poco.

[![Build status](https://ci.appveyor.com/api/projects/status/94932v6vx6mp2g57?svg=true)](https://ci.appveyor.com/project/TimGeyssens/uiomatic)
[![NuGet release](https://img.shields.io/nuget/v/Nibble.Umbraco.UIOMatic.svg)](https://www.nuget.org/packages/Nibble.Umbraco.UIOMatic)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/developer-tools/ui-o-matic/)


## How can I install it? ##
UI-O-Matic can be installed from the [Nuget package repository](https://www.nuget.org/packages/Nibble.Umbra
co.UIOMatic/), or build manually from the [source-code](https://github.com/TimGeyssens/UIOMatic)

For Umbraco v7 use v2.latest of UI-O-Matic

For Umbraco v8 use v3.latest of UI-O-Matic

![nuget install](img/nuget.png)

## Getting Started ##
Of course make sure that UI-O-Matic is installed, your user has access to the new UI-O-Matic section and then create your poco

### Example  ###
If you have the following db table

    CREATE TABLE [People] (
      [Id] int IDENTITY (1,1) NOT NULL
    , [FirstName] nvarchar(255) NOT NULL
    , [LastName] nvarchar(255) NOT NULL
    , [Picture] nvarchar(255) NOT NULL
    );

And the following petapoco/npoco poco

    [TableName("People")]
    public class Person
    {

        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Picture { get; set; }

    }

The next additions to the class (attributes)

    [UIOMatic("people","People","Person", FolderIcon = "icon-users", ItemIcon = "icon-user")]
    [TableName("People")]
    public class Person
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

		[Required]
        [UIOMaticField(Name = "First name", Description = "Enter the persons first name")]
        public string FirstName { get; set; }

		[Required]	
        [UIOMaticField(Name = "Last name",Description = "Enter the persons last name")]
        public string LastName { get; set; }

        [UIOMaticField(Name = "Picture",Description = "Select a picture", View =  UIOMatic.Constants.FieldEditors.File)]
        public string Picture { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

    }

will generate the following crud UI

v8
![](img/examplev8.png)

v7
![](img/gettingstartedexample.png)




