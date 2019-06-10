module Fiour.Web.HttpWebResponse.Combinators
open Fiour.Web.HttpWebResponse.Builder

let hwrAnd (x:HWRB<bool>) (y:HWRB<bool>) : HWRB<bool> = 
  fun hwr ->
    match x hwr with
    | false -> false
    | true -> y hwr

let hwrOr (x:HWRB<bool>) (y:HWRB<bool>) : HWRB<bool> = 
  fun hwr ->
    match x hwr with
    | true -> true
    | false -> y hwr