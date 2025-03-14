import Toybox.Lang;

using Toybox.Time;

typedef SimpleAvalancheWarning as {
  "dangerLevel" as Number,
  "validity" as Array,
};

typedef SimpleAvalancheForecast as Array<SimpleAvalancheWarning>;

typedef AvalancheProblem as {
  "typeName" as String,
  "exposedHeights" as Array<Number>,
  "validExpositions" as String,
  "dangerLevel" as Number,
};

typedef DetailedAvalancheWarning as {
  "dangerLevel" as Number,
  "validity" as Array,
  "mainText" as String,
  "avalancheProblems" as Array<AvalancheProblem>,
};

typedef SetupSubscriptionResponse as {
  "status" as String,
  "addWatchKey" as String?,
};
