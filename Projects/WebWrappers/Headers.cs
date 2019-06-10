using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Fiour.Web.Headers
{
  public class Headers :
    IReadOnlyDictionary<string, string>,
    IReadOnlyList<string>
  {
    private readonly WebHeaderCollection Wrapped;

    public string this[string key] =>
      Wrapped[key] ?? throw new KeyNotFoundException();

    public string this[int index] =>
      Wrapped[index] ?? throw new KeyNotFoundException();

    public string this[HttpRequestHeader header] =>
      Wrapped[header] ?? throw new KeyNotFoundException();

    public string this[HttpResponseHeader header] =>
      Wrapped[header] ?? throw new KeyNotFoundException();

    public IEnumerable<string> Keys =>
      Wrapped.Keys.Cast<string>();

    public IEnumerable<string> Values =>
      Keys.Select(k => Wrapped[k]);

    public int Count => Wrapped.Count;

    public Headers(WebHeaderCollection wrapped)
    {
      Wrapped = wrapped;
    }

    public bool ContainsKey(string key) =>
      TryGetValue(key, out string _);

    public bool ContainsKey(HttpRequestHeader header) =>
      TryGetValue(header, out string _);

    public bool ContainsKey(HttpResponseHeader header) =>
      TryGetValue(header, out string _);

    public bool TryGetValue(string key, out string value)
    {
      value = Wrapped[key];
      return value != null;
    }

    public bool TryGetValue(HttpRequestHeader header, out string value)
    {
      value = Wrapped[header];
      return value != null;
    }

    public bool TryGetValue(HttpResponseHeader header, out string value)
    {
      value = Wrapped[header];
      return value != null;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
      Keys
        .Select(k => new KeyValuePair<string, string>(k, Wrapped[k]))
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<string> IEnumerable<string>.GetEnumerator() =>
      Values.GetEnumerator();
  }
}
