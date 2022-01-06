namespace Funtom.Linq

module SpeedOpt =
  [<Struct;NoComparison;NoEquality>]
  type LargeArrayBuilder<'T> =
    member __.foo() = ()

