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

let hasCookieWith pred cookie hwr =
  let cs = cookies hwr
  match cs.TryGetValue(cookie) with
  | (false,_) -> false
  | (true,c) -> pred c

let hasCookie cookie = 
  hasCookieWith (fun _ -> true) cookie

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