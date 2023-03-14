Приложения, реализуют расчет числе фибоначчи.
Описание проектов:

FibonacciCalcApp - консольное приложение. Инициализирует первоначальный расчет, передает значения по web api.
										Подписывается на канал rabbitmq и получается сообщения для расчета.
										После нового расчета передает значение по web api.
					В конфигурационном файле appsettings содержит следуюшие секции конфигов для настройки: 
						"MessageBrokerOptions" - для настройки доступа к брокеру сообщений
						"FibonacciCalcOptions" - настройки количества расчетов
						"FibonacciApiOptions" - настройки подключения к api

FibonacciCalcApi - web api. Получает значение по api, расчитывает следующее, передает другому сервису через rabbitmq
					В конфигурационном файле appsettings содержит секцию 
					"MessageBrokerOptions" для настройки доступа к брокеру сообщений
			
FibonacciCalc.ApiClient - библиотека-клиент для web api

FibonacciCalc - библиотека, содержит общую часть для FibonacciCalcApp и FibonacciCalcApi с логикой хранения последнего значения каждого расчета и 
				получения следующего значения последовательности

FibonacciCalc.Tests - тесты для FibonacciCalc
FibonacciCalcApi.Tests - тесты для web api

Использованные компоненты:
	.Net Core 6.0
	REST - Asp.Net web api + httpclient
	брокер сообщений - rabbitmq + easyqnet
	логирование - serilog
