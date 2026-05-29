# Книжная лавка — C# Windows Forms, .NET Framework 4.8

Эта версия сделана без SDK-style проекта, поэтому ошибка `project.assets.json не найден` не появляется.

## Запуск

1. Откройте `BookShop.sln` в Visual Studio.
2. Выберите `Debug | Any CPU`.
3. Нажмите `F5` или `Ctrl+F5`.

Если Visual Studio попросит установить компоненты, нужен workload **.NET desktop development** и targeting pack **.NET Framework 4.8**.

## Реализовано

- чтение `Authors.txt`, `Genres.txt`, `Books.txt`;
- вывод списка книг в `DataGridView`;
- карточка выбранной книги через `TextBox` и `RadioButton`;
- фильтр по автору и жанру через `ComboBox`;
- меню `MenuStrip`;
- добавление, редактирование и удаление книг через вторую форму;
- изменения выполняются только в коллекции, файлы не перезаписываются.
