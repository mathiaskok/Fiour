module Fiour.Web.HttpWebResponse
open System.Net
open Fiour.Web.Cookies
open Fiour.Web.Headers

type private HWR = HttpWebResponse

let statusCode (hwr:HWR) = 
  hwr.StatusCode

let hasStatusCode code = 
  statusCode >> ((=)code)

let contentType (hwr:HWR) = 
  hwr.ContentType

let hasContentType content = 
  contentType >> ((=)content)

let cookies (hwr:HWR) =
  Cookies(hwr.Cookies)

let getCookiesWith pred hwr =
  let cs = cookies hwr
  Seq.filter pred cs.Values

let getNamedCookieWith pred name hwr = 
  let cs = cookies hwr
  match cs.TryGetValue(name) with
  | (false,_) -> None
  | (true,c) ->
    match pred c with
    | true -> Some c
    | false -> None

let getNamedCookie name = 
  getNamedCookieWith (fun _ -> true) name

let hasCookieWith pred =
  getCookiesWith pred >> Seq.isEmpty >> not

let hasNamedCookieWith pred name =
  getNamedCookieWith pred name >> Option.isSome

let hasNamedCookie name = 
  hasNamedCookieWith (fun _ -> true) name

let headers (hwr:HWR) = 
  Headers(hwr.Headers)

let hasHeaderWith pred (header:string) hwr =
  let hs = headers hwr
  match hs.TryGetValue(header) with
  | (false,_) -> false
  | (true,h) -> pred h

let hasHeaderEq header value =
  hasHeaderWith ((=)value) header

let hasHeader header = 
  hasHeaderWith (fun _ -> true) header