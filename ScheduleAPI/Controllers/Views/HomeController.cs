﻿using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Exceptions.View;
using System.Net;

namespace ScheduleAPI.Controllers.Views
{
    /// <summary>
    /// Класс-контроллер для домашней страницы API.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Базовый контроллер для логирования событий, тесно не связанных со специализированными контроллерами.
        /// </summary>
        public static ILogger<HomeController>? BaseLogger { get; private set; } = null;

        /// <summary>
        /// Конструктор класса. Неявно вызывается при первом обращении к API.
        /// </summary>
        /// <param name="logger">Настроенный сервис логирования.</param>
        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            BaseLogger = logger;
        }

        /// <summary>
        /// Функция получения базовой страницы API.
        /// </summary>
        /// <returns>Запрос на отрисовку страницы "Index.cshtml".</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает какой-либо статусный код при обращении к серверу.
        /// Возвращает представление, соответствующее отправленному HTTP-коду.
        /// </summary>
        /// <param name="code">Статусный код.</param>
        /// <returns>Представление, содержащее данные об отправленном HTTP-коде.</returns>
        public IActionResult Status(int code) =>
               Error(code);

        /// <summary>
        /// Эта функция автоматически срабатывает в случае возникновения НЕ критической ошибки в работе API. <br />
        /// Она перенаправляет пользователя на страницу с информацией.
        /// </summary>
        /// <returns>Страница с информацией об ошибке.</returns>
        public IActionResult Error(int code = 0)
        {
            var model = new ErrorModel()
            {
                RequestID = Guid.NewGuid(),
                ErrorCode = GetElementByCode(code),
                Message = "Happened something bad.\n\nWe'll fix it as soon, as it possible."
            };

            return View("Error", model);
        }

        /// <summary>
        /// Выполняет приведение целочисленного значения HTTP-кода к типу перечисления.
        /// Данное перечисление больше соответствует необходимым требованиям.
        /// <br />
        /// Если соответствующий элемент перечисления не найден, будет возвращён код 403 (Forbidden).
        /// </summary>
        /// <param name="code">Целочисленный HTTP-код (404, 403, 500, т.д.).</param>
        /// <returns>Элемент перечисления, соответствующий отправленному коду.</returns>
        private static HttpStatusCode GetElementByCode(int code)
        {
            if (Enum.IsDefined(typeof(HttpStatusCode), code))
                return (HttpStatusCode)code;

            return HttpStatusCode.Forbidden;
        }
    }
}
