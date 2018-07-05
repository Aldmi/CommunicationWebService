﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Worker.Background.Abstarct;

namespace Worker.Background.Concrete.BackgroundSerialPort
{
    public class BackgroundMasterSerialPort : HostingBackgroundScoped
    {
        #region Field

        private readonly ConcurrentDictionary<int, Func<CancellationToken, Task>> _cycleTimeFuncDict = new ConcurrentDictionary<int, Func<CancellationToken, Task>>();
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _oneTimeFuncQueue = new ConcurrentQueue<Func<CancellationToken, Task>>();

        #endregion





        #region ctor

        public BackgroundMasterSerialPort(IServiceScopeFactory serviceScopeFactory, KeyBackground keyBackground) 
            : base(serviceScopeFactory, keyBackground)
        {
        }

        #endregion





        #region Methode

        /// <summary>
        /// Добавление функций для циклического опроса
        /// </summary>
        public override void AddCycleFunc(Func<CancellationToken, Task> action)
        {
            if (action != null)
                _cycleTimeFuncDict.TryAdd(_cycleTimeFuncDict.Count, action);
        }


        /// <summary>
        /// Удаление функции для циклического опроса
        /// </summary>
        public override void RemoveCycleFunc(Func<CancellationToken, Task> action)
        {
            if (action != null)
            {
                var key = _cycleTimeFuncDict.FirstOrDefault(entry => entry.Value == action).Key;
                _cycleTimeFuncDict.TryRemove(key, out action);
            }
        }


        /// <summary>
        /// Добавление данных для одиночной функции запроса DataExchangeAsync
        /// </summary>
        public override void AddOneTimeFunc(Func<CancellationToken, Task> action)
        {
            if (action != null)
               _oneTimeFuncQueue.Enqueue(action);
        }


        protected override async Task ExecuteInScopeAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var indexCycleFunc = 0;
            //вызов циклических функций--------------------------------------------------------------------
            if (_cycleTimeFuncDict != null && _cycleTimeFuncDict.Count > 0)
            {
                if (indexCycleFunc >= _cycleTimeFuncDict.Count)
                    indexCycleFunc = 0;

                await _cycleTimeFuncDict[indexCycleFunc++](stoppingToken);
            }

            //вызов одиночной функции запроса---------------------------------------------------------------
            if (_oneTimeFuncQueue != null && _oneTimeFuncQueue.Count > 0)
            {
                while (_oneTimeFuncQueue.TryDequeue(out var oneTimeaction))
                {
                    await oneTimeaction(stoppingToken);
                }
            }

            await Task.Delay(2000, stoppingToken);//DEBUG
            Console.WriteLine($"BackGroundMasterSp  {KeyBackground.Key}");//DEBUG
        }

        #endregion
    }


}