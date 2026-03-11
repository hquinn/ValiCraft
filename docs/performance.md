# Performance

ValiCraft delivers exceptional performance compared to traditional validation libraries. The following benchmarks were run on .NET 10 (Apple M1 Pro):

## Collection Validation - Small Collection
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    65.7081 ns |   2.2774 ns |   1.3553 ns |    65.2078 ns | 0.0625 |      - |     392 B |
| ValiCraftWithMetaData |   103.0613 ns |   2.8023 ns |   1.6676 ns |   102.4566 ns | 0.1006 |      - |     632 B |
| FluentValidation      |   234.8659 ns |  89.0557 ns |  58.9048 ns |   204.7604 ns | 0.1006 |      - |     632 B |

## Collection Validation - Large Collection
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    78.5249 ns |   0.6742 ns |   0.4459 ns |    78.4299 ns | 0.0625 |      - |     392 B |
| ValiCraftWithMetaData |   116.3129 ns |   3.7672 ns |   2.2418 ns |   115.4422 ns | 0.1006 |      - |     632 B |
| FluentValidation      |   190.8824 ns |  18.7856 ns |  12.4255 ns |   185.5197 ns | 0.1006 |      - |     632 B |

## Complex Validation - Valid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    15.9069 ns |   0.1611 ns |   0.0842 ns |    15.9133 ns |      - |      - |         - |
| ValiCraftWithMetaData |    15.8980 ns |   0.1148 ns |   0.0683 ns |    15.8920 ns |      - |      - |         - |
| FluentValidation      |   448.5421 ns |  42.2256 ns |  22.0848 ns |   438.6841 ns | 0.1450 |      - |     912 B |

## Complex Validation - Invalid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |   260.4833 ns |  60.4952 ns |  40.0138 ns |   251.0110 ns | 0.2217 | 0.0010 |    1392 B |
| ValiCraftWithMetaData |   321.2787 ns |  76.0265 ns |  50.2868 ns |   307.3249 ns | 0.2599 | 0.0010 |    1632 B |
| FluentValidation      | 8,253.9831 ns | 158.8160 ns |  83.0638 ns | 8,269.4085 ns | 4.1199 | 0.1068 |   25904 B |

## Simple Validation - Valid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |     5.4063 ns |   0.4838 ns |   0.3200 ns |     5.2896 ns |      - |      - |         - |
| ValiCraftWithMetaData |     5.4166 ns |   0.0279 ns |   0.0146 ns |     5.4110 ns |      - |      - |         - |
| FluentValidation      |   168.1272 ns |   2.9380 ns |   1.5366 ns |   167.3779 ns | 0.1097 |      - |     688 B |

## Simple Validation - Invalid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    94.5754 ns |  23.7687 ns |  15.7215 ns |    93.7476 ns | 0.0777 | 0.0001 |     488 B |
| ValiCraftWithMetaData |   148.4748 ns |  19.2683 ns |  12.7448 ns |   146.8852 ns | 0.1160 | 0.0001 |     728 B |
| FluentValidation      | 2,561.6943 ns | 326.4619 ns | 215.9344 ns | 2,516.8830 ns | 1.0681 | 0.0076 |    6712 B |

## Instantiation - Simple Validators
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |     0.0013 ns |   0.0037 ns |   0.0022 ns |     0.0000 ns |      - |      - |         - |
| ValiCraftWithMetaData |     0.0000 ns |   0.0000 ns |   0.0000 ns |     0.0000 ns |      - |      - |         - |
| FluentValidation      | 1,889.3677 ns | 653.7875 ns | 432.4400 ns | 1,628.5814 ns | 1.0376 |      - |    6568 B |

## Instantiation - Complex Validators
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |     0.0024 ns |   0.0068 ns |   0.0040 ns |     0.0000 ns |      - |      - |         - |
| ValiCraftWithMetaData |     0.0000 ns |   0.0000 ns |   0.0000 ns |     0.0000 ns |      - |      - |         - |
| FluentValidation      | 6,483.3496 ns | 329.5673 ns | 196.1202 ns | 6,460.3780 ns | 3.9063 | 0.1221 |   24664 B |

> **Note:** ValiCraft validators have zero instantiation cost because the source generator produces pure static validation code with no runtime initialization.
