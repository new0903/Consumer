
Для запуска надо установить файлы yolov3.cfg	yolov3.weights	coco.names 
https://github.com/AlturosDestinations/Alturos.Yolo?tab=readme-ov-file
Перенести эти файлы в WinFormsApp1\bin\Debug\net7.0-windows
В проекте Producer(отправитель) указать ip Consumer(получателя - WinFormsApp1). Затем запустить Producer и потом WinFormsApp1. Producer отправляет изображение в байтах с камеры WinFormsApp1. WinFormsApp1 Загружает изображение и обрабатывает его на наличие объектов(людей, домашних животных, машин и т.д. полный список опозноваемых объектов в файле yolov3.cfg). Получается очень прикольная камера с распознованием объектов)))