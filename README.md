# CustomRichTextBlock

Custom RichTextBlock control for Universal Windows Platform that is able to
translate specific plain text into emoji image, hyperlink button, etc.

![Demo](https://github.com/jasonwun/CustomRichTextBlock/blob/master/CustomRichTextBlock/Assets/CustomRichTextBlock.gif)


## Feature 
 
 1. Only need to input the plain text in the property of Text.
 
 2. Text property bindable.





## Known issue
 
 The control will not trigger the textchanged event if the plain text is directly input in Text property. You shall use binding to bind the plain text to the control.
