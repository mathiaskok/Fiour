using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Fiour.Web.Cookies
{
  public class Cookies :
    IReadOnlyDictionary<string, Cookie>,
    IReadOnlyList<Cookie>
  {
    private readonly CookieCollection Wrapped;

    public Cookie this[string key] =>
      Wrapped[key] ?? throw new KeyNotFoundException();

    public Cookie this[int index] =>
      Wrapped[index] ?? throw new ArgumentOutOfRangeException();

    public IEnumerable<string> Keys =>
      AsCookiesEnumerable()
      .Select(c => c.Name);

    public IEnumerable<Cookie> Values =>
      AsCookiesEnumerable();

    public int Count => Wrapped.Count;

    public Cookies(CookieCollection wrapped)
    {
      Wrapped = wrapped;
    }

    public bool ContainsKey(string key) =>
      TryGetValue(key, out Cookie c);

    public bool TryGetValue(string key, out Cookie value)
    {
      value = Wrapped[key];
      return value != null;
    }

    public IEnumerator<Cookie> GetEnumerator() =>
      AsCookiesEnumerable()
        .GetEnumerator();

    IEnumerator<KeyValuePair<string, Cookie>> IEnumerable<KeyValuePair<string, Cookie>>.GetEnumerator() =>
      AsCookiesEnumerable()
        .Select(c => new KeyValuePair<string, Cookie>(c.Name, c))
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
      AsCookiesEnumerable().GetEnumerator();

    private IEnumerable<Cookie> AsCookiesEnumerable() =>
      Wrapped.Cast<Cookie>();
  }
}