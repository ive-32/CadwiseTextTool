﻿## Бенчмаркинг обоработки текста

Коль взялись оптимизировать метод RefineOneString подключим бенчмарк,
чтобы измерять насколько оптимизировали.

Делает бенчмарк методов RefineOneString() до оптимизации и после. 

Также делает бенчмарк низкоуровневого метода RefineOneWord()

В качестве оптимизированного метода берется текущий из проекта 
CadwiseTextTool

Старая версия метода внедрена в проект BenchMarking для сравнения.

| Method                | Mean       | Error    | StdDev   | Gen0     | Allocated |
|-----------------------|------------|----------|----------|----------|-----------|
| TestOldRefinement     | 1,175.4 μs | 19.04 μs | 15.90 μs | 189.4531 | 290.39 KB |
| TestNewRefinement     | 491.7 μs   | 5.92 μs  | 4.94 μs  | 148.4375 | 228.43 KB |
| TestOneWordRefinement | 276.2 μs   | 3.61 μs  | 3.37 μs  | 49.3164  | 76.22 KB  |

Не скажу, что много выиграл по памяти: <br>
сборка мусора сократилась до 148 с 189 на 1000 запусков<br>
памяти нужно 228Kb, а было 290Kb

по скорости - лучше: в два раза быстрее стало. 