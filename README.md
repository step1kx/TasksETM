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

| Начальное окно (и его вариации) |       

![mainFirst](https://github.com/step1kx/TasksETM/raw/master/screenshots/startWindowDepartment.png)    

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/startWindowGIP.png)      

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/startWindowComboBox.png)     

| Окно выбора проекта |    

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/chooseProjectsWindow.png)   

| Окно отображения задания |

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/taskWindow.png)

| Окно создания заданий |

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/taskCreateWindowFirst.png)  

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/taskCreateWindowSecond.png)

| Окно фильтрации заданий |

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/taskFilteredWindow.png)  

![mainSecond](https://github.com/step1kx/TasksETM/raw/master/screenshots/taskFilteredWindowSecond.png)


---

## Будущие планы и реализации

- Сделать приложение, которое будет совмещать все проекты пакеты "ETMSoftware", для удобной установки и работы с ними;
- Корректировка данного приложения от замечаний и предложений ведущих специалистов компании ETM;
- Добавление и реализация новых идей (автоматические обновление программы при моих изменениях, новые способы уведомлять сотрудников о заданиях и т.д.).


