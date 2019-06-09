module Fiour.Retry
open System
open System.Threading.Tasks

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

type RetryException = 
  inherit Exception
  new () = {inherit Exception()}
  new (msg) = {inherit Exception(msg)}
  new (msg,exn:Exception) = {inherit Exception(msg,exn)}

let private transformWaitFunc (waitFunc:Func<int,Task>) =
  waitFunc.Invoke >> Async.AwaitTask

let private transformRetryErrFunc (retryErrFunc:Action<RetryException>) = 
  retryErrFunc.Invoke

let private transformMainFunc (mainFunc:Func<'a>) =
  fun () ->
    try mainFunc.Invoke() |> Ok
    with 
      | :? RetryException as ex -> Error ex



let private transformRetryRes res = Async.StartAsTask <| async{
  let! a = res
  return Option.toObj a
}

[<AbstractClass; Sealed>]
type Interop private () =
  static member WithRetryAsync (waitFunc,retryErrFunc,mainFunc,limit) =
    transformRetryRes <| 
      withRetryAsync
        (transformWaitFunc waitFunc)
        (transformRetryErrFunc retryErrFunc)
        (transformMainFunc mainFunc)
        limit

  static member RetryConstantBackoff (retryErrFunc,mainFunc,limit,backoff) =
    transformRetryRes <|
      retryConstantBackoff
        (transformRetryErrFunc retryErrFunc)
        (transformMainFunc mainFunc)
        limit
        backoff

  static member RetryLinearBackoff (retryErrFunc,mainFunc,limit,backoff) =
    transformRetryRes <|
      retryLinearBackoff
        (transformRetryErrFunc retryErrFunc)
        (transformMainFunc mainFunc)
        limit
        backoff

  static member RetryExponentialBackoff (retryErrFunc,mainFunc,limit,backoff) =
    transformRetryRes <|
      retryExponentialBackoff
        (transformRetryErrFunc retryErrFunc)
        (transformMainFunc mainFunc)
        limit
        backoff