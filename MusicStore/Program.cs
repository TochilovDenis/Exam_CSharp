﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using MusicStore;

class Program
{
    static void Main()
    {
        // Инициализация менеджеров
        var storeManager = new StoreManager("YourConnectionString");
        var customerManager = new CustomerManager("YourConnectionString");
        var promotionManager = new PromotionManager("YourConnectionString");

        // Пример добавления пластинки
        var album = new Album
        {
            Title = "Новый альбом",
            Artist = "Группа",
            Label = "Студия",
            TrackCount = 12,
            GenreID = 1,
            YearReleased = 2025,
            CostPrice = 500.00m,
            SellingPrice = 800.00m
        };

        storeManager.AddAlbum(album);

        // Поиск пластинок
        var results = storeManager.SearchAlbums("рок");

        // Регистрация клиента
        var customer = new Customer
        {
            Login = "user123",
            PasswordHash = "password123"
        };

        customerManager.RegisterCustomer(customer);
    }

   
}

/*Создать приложение «Музыкальный магазин».  
Основная задача проекта: учитывать текущий ассортимент музыкальных пластинок в магазине. 

Необходимо хранить следующую информацию о пластинках:
 - название пластинки,
 - название коллектива,
 - названиеи здателя,
 - количество треков,
 - жанр, 
 - год издания,
 - себестоимость, 
 - цена для продажи. 

Приложение должно позволять:
 - добавлять пластинки,
 - удалятьпластинки, 
 - редактировать параметры пластинок,
 - продавать пластинки,
 - списывать пластинки,
 - вносить пластинки в акции (например, неделя пластинок джаза со скидкой 10%), 
 - откладывать диски для конкретного покупателя. 

Приложение должно предоставить функциональность по поиску дисков потоком параметрам: 
 - название диска, 
 - исполнитель,
 - жанр.

Приложение должно предоставлять возможность просмотреть 
 - список новинок, 
 - список самых продаваемых пластинок,
 - список самых популярных авторов,
 - список самых популярных жанров по итогам дня, недели, месяца, года. 

Необходимо предусмотреть возможность входа по логину и паролю.
Также нужно сделать возможность регистрации постоянных покупателей и создать 
систему скидок в зависимости от накопленной суммы потраченных средств.*/