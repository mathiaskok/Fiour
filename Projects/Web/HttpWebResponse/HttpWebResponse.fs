module Fiour.Web.HttpWebResponse
open System.Net
open Fiour.Web.Cookies
open Fiour.Web.Headers

type internal HWR = HttpWebResponse

let private constTrue _ = true

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
  getNamedCookieWith constTrue name

let hasCookieWith pred =
  getCookiesWith pred >> Seq.isEmpty >> not

let hasNamedCookieWith pred name =
  getNamedCookieWith pred name >> Option.isSome

let hasNamedCookie name = 
  hasNamedCookieWith constTrue name

let headers (hwr:HWR) = 
  Headers(hwr.Headers)

let getHeader (header:string) hwr =
  let hs = headers hwr
  match hs.TryGetValue(header) with
  | (false,_) -> None
  | (true,h) -> Some h

let getHeaderEnum (header:HttpResponseHeader) hwr =
  let hs = headers hwr
  match hs.TryGetValue(header) with
  | (false,_) -> None
  | (true,h) -> Some h

let hasHeaderWith pred header hwr =
  match getHeader header hwr with
  | None -> false
  | Some h -> pred h

let hasHeaderEnumWith pred header hwr =
  match getHeaderEnum header hwr with
  | None -> false
  | Some h -> pred h

let hasHeaderEq header value =
  hasHeaderWith ((=)value) header

let hasHeaderEnumEq header value =
  hasHeaderEnumWith ((=)value) header

let hasHeader header =
  hasHeaderWith constTrue header

let hasHeaderEnum header =
  hasHeaderEnumWith constTrue header

let transformAndCloseContent trans (hwr:HWR) =
  use stream = hwr.GetResponseStream()
  trans stream

let transformAndCloseResponse trans (hwr:HWR) = 
  use response = hwr
  trans response