#  TasksETM

**TasksETM** — это настольное WPF-приложение для управления задачами внутри организации.   
Оно позволяет создавать, отслеживать и выполнять задачи.  
Одно из первых настольных приложение под Windows для локального пользования в компании ETM.  
Явлется первым приложение, которое будет входить в пакет приложение "ETMSoftware" (в будущем планируется написания большего числа программ).

## Реализация

- Приложение реализовано с помощью языка C# и WPF;
- База данных, используемая в проекте - PostgreSQL;
- Приложение придерживается принципов ООП, МVVM и Clean Architecture.

---

## Возможности

-  Создание и редактирование задач;
-  Автоматическое выставление даты создания;
-  Указание даты выполнения (в формате `ДД.ММ.ГГГГ`);
-  Поиск и фильтрация по задачам;
-  Окно справки с подсказками и инструкциями.

---

## Скриншоты работы программы

<h3 align="center">Начальное окно (и его вариации)</h3>

<p align="center">
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/startWindowDepartment.png" width="600"/>
  <br/>
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/startWindowGIP.png" width="600"/>
  <br/>
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/startWindowComboBox.png" width="600"/>
</p>

<h3 align="center">Окно выбора проекта</h3>

<p align="center">
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/chooseProjectsWindow.png" width="600"/>
</p>

<h3 align="center">Окно отображения задания</h3>

<p align="center">
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/taskWindow.png" width="600"/>
</p>

<h3 align="center">Окно создания заданий</h3>

<p align="center">
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/taskCreateWindowFirst.png" width="600"/>
  <br/>
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/taskCreateWindowSecond.png" width="600"/>
</p>

<h3 align="center">Окно фильтрации заданий</h3>

<p align="center">
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/taskFilteredWindow.png" width="600"/>
  <br/>
  <img src="https://github.com/step1kx/TasksETM/raw/master/screenshots/taskFilteredWindowSecond.png" width="600"/>
</p>

---

## Будущие планы и реализации

- Сделать приложение, которое будет совмещать все проекты пакеты "ETMSoftware", для удобной установки и работы с ними;
- Корректировка данного приложения от замечаний и предложений ведущих специалистов компании ETM;
- Добавление и реализация новых идей (автоматические обновление программы при моих изменениях, новые способы уведомлять сотрудников о заданиях и т.д.).


