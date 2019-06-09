module WebBehaviors.Retry
open System

let private timeSpanToMil (time:TimeSpan) =
  int (round time.TotalMilliseconds)

let private asyncSleepTimeSpan (wait:TimeSpan) = async{
  do! Async.Sleep (timeSpanToMil wait)
}

let timeSpanMul (time:TimeSpan) mul = 
  let longMul = int64 mul
  TimeSpan.FromTicks(time.Ticks * longMul);

let timeSpanPower (time:TimeSpan) mul =
  let wait = pown time.Ticks mul
  TimeSpan.FromTicks wait

let constantBackoff wait = async{
  do! asyncSleepTimeSpan wait
}
  
let linearBackoff wait mul = async{
  do! asyncSleepTimeSpan (timeSpanMul wait mul)
}

let exponentialBackoff wait mul = async{
  do! asyncSleepTimeSpan (timeSpanPower wait mul)
}

let rec private withRetryAsyncInt attemptNr waitFunc retryErrFunc mainFunc limit = async{
  match attemptNr > limit with
  | true -> return None
  | false  -> 
    let res = mainFunc()
    match res with
    | Ok a -> return Some a
    | Error err -> 
      do retryErrFunc err
      do! waitFunc (attemptNr+1)
      return! withRetryAsyncInt (attemptNr+1) waitFunc retryErrFunc mainFunc limit
}

let withRetryAsync waitFunc retryErrFunc mainFunc limit = 
  withRetryAsyncInt 0 waitFunc retryErrFunc mainFunc limit

let retryConstantBackoff retryErrFunc mainFunc limit backoff = 
  withRetryAsync (fun _ -> constantBackoff backoff) retryErrFunc mainFunc limit

let retryLinearBackoff retryErrFunc mainFunc limit backoff = 
  withRetryAsync (linearBackoff backoff) retryErrFunc mainFunc limit

let retryExponentialBackoff retryErrFunc mainFunc limit backoff = 
  withRetryAsync (exponentialBackoff backoff) retryErrFunc mainFunc limit