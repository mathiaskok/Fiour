module Fiour.Web.HttpWebResponse.Builder
open Fiour.Web.HttpWebResponse

type internal HWRB<'a> = HWR -> 'a

let ret a : HWRB<'a> = fun _ -> a

let map func (hwrb:HWRB<'a>) : HWRB<'b> = 
  hwrb >> func

let apply (func:HWRB<'a -> 'b>) (x:HWRB<'a>) : HWRB<'b> = 
  fun hwr ->
    let f = func hwr
    let a = x hwr
    f a

let bind (x:HWRB<'a>) (binder: 'a -> HWRB<'b>) : HWRB<'b> = 
  fun hwr ->
    let a = x hwr
    binder a hwr

let join (x:HWRB<HWRB<'a>>) : HWRB<'a> =
  fun hwr ->
    let f = x hwr
    f hwr

type HttpWebResponseBuilder() =
  member __.Return(x) = ret x
  member __.ReturnFrom(hwrb) = hwrb
  member __.Bind(x,binder) = bind x binder

let httpResponse = HttpWebResponseBuilder()