﻿Со временем увеличивается скорость и интенсивность появления кругов
В проекте не использованы Unity Bundle из за отсутствия Pro версии
Уровень меняется каждые 5 отсчетов времени (Timer:0 - 0 уровень, Timer:5 - 1 уровень)
При смене уровня создается отдельный поток, который начинает рисовать новые текстуры (сет) для следующего уровня - 4 шт. для разрешений: 32*32, 64*64, 128*128, 256*256
Круги одного сета текстур имеют один и тот же градиент
В правом нижнем углу экрана дополнительно показано число живых сетов
Все неиспользуемые сеты чистятся - для этого использован счетчик ссылок на конкретный сет
