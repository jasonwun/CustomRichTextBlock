# CustomRichTextBlock

Custom RichTextBlock control for Universal Windows Platform that is able to
translate specific plain text into emoji image, hyperlink button, etc.

![Demo](https://github.com/jasonwun/CustomRichTextBlock/blob/master/CustomRichTextBlock/Assets/CustomRichTextBlock.gif)


## Feature 
 
 1. Only need to input the plain text in the property of Text.
 
 2. Text property bindable.





## Known issue
 
 Input with plain text directly will cause xaml reader throws exception. The only workaround here is using binding on the control, just like how I do on the project.
