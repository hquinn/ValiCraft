# Performance

ValiCraft delivers exceptional performance compared to traditional validation libraries. The following benchmarks were run on .NET 10 (Apple M1 Pro):

## Simple Validation - Valid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    19.1820 ns |   2.6256 ns |   1.7367 ns |    18.5920 ns |      - |      - |         - |
| ValiCraftWithMetaData |    12.7780 ns |   1.0308 ns |   0.6134 ns |    12.8810 ns |      - |      - |         - |
| FluentValidation      |   168.0000 ns |   0.8838 ns |   0.5846 ns |   167.9520 ns | 0.1097 |      - |     688 B |

## Simple Validation - Invalid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    62.7240 ns |   0.7155 ns |   0.4258 ns |    62.7440 ns | 0.0854 | 0.0002 |     536 B |
| ValiCraftWithMetaData |    98.6020 ns |   0.7956 ns |   0.5262 ns |    98.7350 ns | 0.1236 | 0.0006 |     776 B |
| FluentValidation      | 2,035.4290 ns |   7.5696 ns |   5.0068 ns | 2,034.0000 ns | 1.0681 | 0.0076 |   6,712 B |

## Complex Validation - Valid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    33.3700 ns |   4.8037 ns |   2.8586 ns |    33.5700 ns |      - |      - |         - |
| ValiCraftWithMetaData |    41.1700 ns |   8.2083 ns |   4.8846 ns |    42.6660 ns |      - |      - |         - |
| FluentValidation      |   428.5660 ns |   2.5883 ns |   1.5403 ns |   429.0770 ns | 0.1450 |      - |     912 B |

## Complex Validation - Invalid Model
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |   202.9070 ns |   0.8482 ns |   0.5048 ns |   202.8320 ns | 0.2944 | 0.0041 |   1,848 B |
| ValiCraftWithMetaData |   240.7930 ns |   4.0169 ns |   2.6569 ns |   240.6350 ns | 0.3324 | 0.0048 |   2,088 B |
| FluentValidation      | 8,202.1920 ns | 302.6695 ns | 200.1971 ns | 8,178.0000 ns | 4.1199 | 0.1068 |  25,904 B |

## Collection Validation - Small Collection
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    54.8540 ns |   0.3206 ns |   0.1908 ns |    54.8840 ns | 0.0803 | 0.0003 |     504 B |
| ValiCraftWithMetaData |    87.5880 ns |   0.6332 ns |   0.3768 ns |    87.5320 ns | 0.1185 | 0.0005 |     744 B |
| FluentValidation      |   179.2400 ns |   1.3696 ns |   0.8150 ns |   179.3990 ns | 0.1006 |      - |     632 B |

## Collection Validation - Large Collection
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |    72.5520 ns |   4.6643 ns |   3.0852 ns |    70.7000 ns | 0.0802 | 0.0002 |     504 B |
| ValiCraftWithMetaData |   104.9000 ns |   1.1399 ns |   0.7540 ns |   104.6430 ns | 0.1185 | 0.0005 |     744 B |
| FluentValidation      |   181.7270 ns |   1.3278 ns |   0.7902 ns |   181.9510 ns | 0.1006 |      - |     632 B |

## Instantiation - Simple Validators
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |     2.7780 ns |   0.0422 ns |   0.0279 ns |     2.7720 ns | 0.0038 |      - |      24 B |
| ValiCraftWithMetaData |     2.8450 ns |   0.0353 ns |   0.0234 ns |     2.8520 ns | 0.0038 |      - |      24 B |
| FluentValidation      | 1,632.2290 ns |  38.6983 ns |  25.5965 ns | 1,646.0000 ns | 1.0452 | 0.0076 |   6,568 B |

## Instantiation - Complex Validators
| Method                | Mean          | Error       | StdDev      | Median        | Gen0   | Gen1   | Allocated |
|---------------------- |--------------:|------------:|------------:|--------------:|-------:|-------:|----------:|
| ValiCraft             |     2.8590 ns |   0.0565 ns |   0.0295 ns |     2.8560 ns | 0.0038 |      - |      24 B |
| ValiCraftWithMetaData |     2.9340 ns |   0.1303 ns |   0.0862 ns |     2.8850 ns | 0.0038 |      - |      24 B |
| FluentValidation      | 6,446.2130 ns |  73.1968 ns |  38.2833 ns | 6,441.0000 ns | 3.9063 | 0.1221 |  24,664 B |

> **Note:** ValiCraft validators have near-zero instantiation cost (~3ns, 24B for the class instance) because the source generator produces validation logic that requires no runtime initialization, unlike FluentValidation which builds expression trees and rule chains at construction time.
