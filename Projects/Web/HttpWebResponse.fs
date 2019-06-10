module Fiour.Web.HttpWebResponse
open System.Net
open Fiour.Web.Cookies

type private HWR = HttpWebResponse

let statusCode (hwr:HWR) = 
  hwr.StatusCode

let hasStatusCode code = 
  statusCode >> ((=)code)

let contentType (hwr:HWR) = 
  hwr.ContentType

let hasContentType content = contentType >> ((=)content)

let cookies (hwr:HWR) =
  Cookies(hwr.Cookies)

let hasCookieWith pred cookie hwr =
  let cs = cookies hwr
  match cs.TryGetValue(cookie) with
  | (false,_) -> false
  | (true,c) -> pred c

let hasCookie cookie = 
  hasCookieWith (fun _ -> true) cookie
