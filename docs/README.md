---
description: The official home of the documentation for the umbraco package UI-O-Matic
---

# UI-O-Matic Docs

UI-O-Matic allows you to auto generate an integrated crud UI in Umbraco v7/v8/v9/v10 for a db table based on a [petapoco](http://www.toptensoftware.com/petapoco/) / [npoco](https://github.com/schotime/NPoco) (the default ORM in Umbraco) poco and more...

Please be aware of the[ License Model](broken-reference) before using this package since additional charges might apply if you are an Umbraco Gold Partner.

[![Build status](https://ci.appveyor.com/api/projects/status/94932v6vx6mp2g57?svg=true)](https://ci.appveyor.com/project/TimGeyssens/uiomatic) [![NuGet release](https://img.shields.io/nuget/v/Nibble.Umbraco.UIOMatic.svg)](https://www.nuget.org/packages/Nibble.Umbraco.UIOMatic)

## How can I install it?&#x20;

UI-O-Matic can be installed from the [Nuget package repository](https://www.nuget.org/packages/Nibble.Umbra%20co.UIOMatic/), or build manually from the [source-code](https://github.com/TimGeyssens/UIOMatic)

For Umbraco **v10** use **v5.latest** of UI-O-Matic

For Umbraco **v9** use **v4.latest** of UI-O-Matic

For Umbraco **v8** use **v3.latest** of UI-O-Matic

For Umbraco **v7** use **v2.latest** of UI-O-Matic

## Getting Started&#x20;

Of course make sure that UI-O-Matic is installed, your user has access to the new UI-O-Matic section and then create your poco

### Example&#x20;

If you have the following db table

```
CREATE TABLE [People] (
  [Id] int IDENTITY (1,1) NOT NULL
, [FirstName] nvarchar(255) NOT NULL
, [LastName] nvarchar(255) NOT NULL
, [Picture] nvarchar(255) NOT NULL
);
```

And the following petapoco/npoco poco

```
[TableName("People")]
public class Person
{

    [PrimaryKeyColumn(AutoIncrement = true)]
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Picture { get; set; }

}
```

The next additions to the class (attributes)

```
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
```

will generate the following crud UI

v8&#x20;

![](broken-reference)

v7&#x20;

![](broken-reference)
