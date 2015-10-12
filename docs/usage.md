# Usage #

With a db table and a petapoco poco in place you'll have a couple of steps to follow


## Decorate your class with the UIOMatic attribute ##

In order for UI-O-Matic to pick up your poco you'll have to mark it with the *UIOMatic* attribute

	[UIOMatic("People","icon-users","icon-user")]

The UIOMatic attribute has a contructor with 3 parameters
	
- Name of the tree
- Icon used for the main tree node
- Icon used for the tree item nodes

## Decorate properties with the UIOMaticField attribute ##

If your properties aren't marked with the *UIOMaticField* attribute, UI-O-Matic will display the property name as the label and also select a view based on the property type. If you wish to have more control you can mark your properties with the *UIOMaticField* attribute.

	 [UIOMaticField("Firstname","Enter your firstname")]

The attribute has a constructor with 2 parameters

- The name of the field (will be shown as the label for the field)
- The description of the field

Optionally it's also possible to specify a view

	[UIOMaticField("Picture", "Please select a picture",View ="file")]

There are a couple out of the box views you can use

- checkbox
- date
- datetime
- file
- label
- number
- password
- pickers.content
- textarea
- textfield

Besides the out of the box ones you can also use a completely custom one 

 	[UIOMaticField("Owner", "Select the owner of the dog", View = "~/App_Plugins/Example/picker.person.html")]

## Decorate properties with the UIOMaticIgnoreField attribute ##

If you wish that UI-O-Matic ignores specific properties (so doesn't display then in the editor) you can mark those with the *UIOMaticIgnoreField* attribute

	[UIOMaticIgnoreField]





