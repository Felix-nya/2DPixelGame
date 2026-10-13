# Auto-Battler

[English](#english) · [Русский](#русский)

---

## English

A two-player auto-battler made in Unity. Players take turns picking heroes, the heroes fight on their own in real time, and after every battle each player chooses a reward to strengthen their team. The player who still has lives left wins.

> **Status:** early prototype. There are placeholders instead of sprites, and balance is not tuned yet.

<img width="1578" height="883" alt="{DEEB109B-1604-45F5-B7F1-03779B626852}" src="https://github.com/user-attachments/assets/5c77dc75-09ef-4018-9679-ece6f1c0e588" />

### Download

Get the latest version from the [Releases](../../releases) page: download the `.zip`, extract it **completely**, and run the `.exe`.

Requirements: Windows 10/11 (64-bit). On first launch Windows SmartScreen may warn about an unknown publisher because the game is not signed. Click "More info" → "Run anyway".

### How to play

1. Two players sit at the same computer and take turns. The game uses the mouse only.
2. Each player picks a starting hero from a random selection.
3. The battle runs automatically in real time. You can speed it up with the speed button. A team wins by defeating all enemy heroes; if time runs out, the team with more health left wins.
4. The player who loses a battle loses one of their three lives.
5. After each battle, every player picks 1 reward out of 3 (the loser sees 4): a stat boost, a new hero, or an item. A hero can hold only one copy of each item.
6. The game ends when one player runs out of lives.

### Features

- 4 heroes with unique abilities and critical hits
- Different targeting strategies for heroes
- Rewards: stat boosts, new heroes, items with effects (thorns, vampirism, rage)
- Floating damage numbers, battle timer, x2 battle speed
- Easy to extend: heroes, abilities, rewards, and items are Unity assets

### Known issues

- No sprites or animations yet (placeholders)
- Balance is not tuned

### Feedback

I'd really like to hear your feedback: what was fun, what was confusing, and what felt too strong or too weak. Please open an [Issue](../../issues). If something broke, tell me what you were doing when it happened.

### Build from source

1. Clone the repository.
2. Open the project in Unity 6.3.7f1.
3. Open `Assets/Scenes/MainMenu.unity` and press Play.

### Tech

Unity 6.3.7f1, C#, .NET Standart 2.1.

### Roadmap

- Sprites and animations
- New heroes and rewards
- Balance tuning and a battle simulation mode
- Sound and music
- Single-player mode against the computer

### Team

- Oleg: programming, balance
- Vitaliy: game design, art

## Русский

Автобаттлер для двух игроков, сделанный на Unity. Игроки по очереди выбирают героев, герои сами сражаются в реальном времени, а после каждого боя каждый игрок выбирает награду и усиливает свою команду. Побеждает тот, у кого остались жизни.

> **Статус:** ранний прототип. Вместо спрайтов пока заглушки, баланс не настроен.

<img width="1578" height="883" alt="{DEEB109B-1604-45F5-B7F1-03779B626852}" src="https://github.com/user-attachments/assets/2b9981be-fb9d-48b9-b952-5a1e4f7944ea" />


### Скачать

Последняя версия лежит в разделе [Releases](../../releases): скачайте `.zip`, распакуйте его **целиком** и запустите `.exe`.

Требования: Windows 10/11 (64 бита). При первом запуске Windows SmartScreen может предупредить о неизвестном издателе, потому что игра не подписана. Нажмите «Подробнее» → «Выполнить в любом случае».

### Как играть

1. Двое игроков сидят за одним компьютером и ходят по очереди. Управление только мышью.
2. Каждый игрок выбирает стартового героя из случайного набора.
3. Бой идёт автоматически в реальном времени, его можно ускорить кнопкой скорости. Команда побеждает, уничтожив всех вражеских героев, а если время вышло, побеждает команда с большим остатком здоровья.
4. Проигравший бой теряет одну из трёх жизней.
5. После боя каждый игрок выбирает 1 награду из 3 (проигравшему показывают 4): усиление, нового героя или предмет. Один предмет нельзя дать герою дважды.
6. Игра заканчивается, когда у одного из игроков не остаётся жизней.

### Что внутри

- 4 героя с уникальными способностями и критическими ударами
- Разные стратегии выбора цели у героев
- Награды: усиления, новые герои, предметы с эффектами (шипы, вампиризм, ярость)
- Всплывающие числа урона, таймер боя, ускорение боя в два раза
- Легко расширять: герои, способности, награды и предметы это ассеты Unity

### Известные проблемы

- Пока нет спрайтов и анимаций (вместо них заглушки)
- Баланс не настроен

### Обратная связь

Мне очень важны отзывы: что понравилось, что было непонятно, что показалось слишком сильным или слабым. Пожалуйста, создайте [Issue](../../issues). Если что-то сломалось, напишите, что вы делали в этот момент.

### Сборка из исходников

1. Клонируйте репозиторий.
2. Откройте проект в Unity 6.3.7f1.
3. Откройте сцену `Assets/Scenes/MainMenu.unity` и нажмите Play.

### Технологии

Unity 6.3.7f1, C#, .NET Standart 2.1.

### Планы

- Спрайты и анимации
- Новые герои и награды
- Настройка баланса и режим симуляции боёв
- Звук и музыка
- Одиночный режим против компьютера

### Команда

- Олег: программирование, баланс
- Виталий: гейм-дизайн, графика
