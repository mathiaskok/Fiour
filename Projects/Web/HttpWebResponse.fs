module Fiour.Web.HttpWebResponse
open System.Net

type private HWR = HttpWebResponse

let statusCode (hwr:HWR) = 
  hwr.StatusCode

let hasStatusCode code = 
  statusCode >> ((=)code)