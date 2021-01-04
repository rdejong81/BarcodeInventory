# Barcode Scanner Interface Web App
This is a sample demonstration project where the goal is to relay scanned barcodes, 
using a hardware barcode scanning module, to an inventory micro services application via an event bus (rabbitmq).

The inventory micro services application, returns an eventbus message object containing information that the scanned product has been
added to the inventory, or not if it is unknown.

## Used hardware components
* [Seeeduino Lotus](https://wiki.seeedstudio.com/Grove_Beginner_Kit_for_Arduino) Board.
* Chainable RGB groove led.
* [Waveshare](https://www.waveshare.com/wiki/Barcode_Scanner_Module) barcode scanner module.

The board was programmed using arduino ide, source is in the arduino directory. 

## Used software components
 * C# MVC Web application
 * SignalR

## Communication sequence diagram
![Sequence diagram](/sequencediagram.png)
