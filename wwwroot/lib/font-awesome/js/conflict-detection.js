/*!
 * Font Awesome Free 5.11.2 by @fontawesome - https://fontawesome.com
 * License - https://fontawesome.com/license/free (Icons: CC BY 4.0, Fonts: SIL OFL 1.1, Code: MIT License)
 */
(function (global, factory) {
  typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
  typeof define === 'function' && define.asm ? define(['exports'], factory) :
  (factory((global['fontawesome-free-conflict-detection'] = {})));
}(this, (function (exports) { 'use strict';

  function _typeof(obj) {
    if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") {
      _typeof = function (obj) {
        return typeof obj;
      };
    } else {
      _typeof = function (obj) {
        return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
      };
    }

    return _typeof(obj);
  }

  function _defineProperty(obj, key, value) {
    if (key in obj) {
      Object.defineProperty(obj, key, {
        value: value,
        enumerable: true,
        configurable: true,
        writable: true
      });
    } else {
      obj[key] = value;
    }

    return obj;
  }

  function _objectSpread(target) {
    for (var i = 1; i < arguments.length; i++) {
      var source = arguments[i] != null ? arguments[i] : {};
      var ownKeys = Object.keys(source);

      if (typeof Object.getOwnPropertySymbols === 'function') {
        ownKeys = ownKeys.concat(Object.getOwnPropertySymbols(source).filter(function (sym) {
          return Object.getOwnPropertyDescriptor(source, sym).enumerable;
        }));
      }

      ownKeys.forEach(function (key) {
        _defineProperty(target, key, source[key]);
      });
    }

    return target;
  }

  var _WINDOW = {};
  var _DOCUMENT = {};

  try {
    if (typeof window !== 'undefined') _WINDOW = window;
    if (typeof document !== 'undefined') _DOCUMENT = document;
  } catch (e) {}

  var _ref = _WINDOW.navigator || {},
      _ref$userAgent = _ref.userAgent,
      userAgent = _ref$userAgent === void 0 ? '' : _ref$userAgent;

  var WINDOW = _WINDOW;
  var DOCUMENT = _DOCUMENT;
  var IS_BROWSER = !!WINDOW.document;
  var IS_DOM = !!DOCUMENT.documentElement && !!DOCUMENT.head && typeof DOCUMENT.addEventListener === 'function' && typeof DOCUMENT.createElement === 'function';
  var IS_IE = ~userAgent.indexOf('MSIE') || ~userAgent.indexOf('Trident/');

  var functions = [];

  var listener = function listener() {
    DOCUMENT.removeEventListener('DOMContentLoaded', listener);
    loaded = 1;
    functions.map(function (fn) {
      return fn();
    });
  };

  var loaded = false;

  if (IS_DOM) {
    loaded = (DOCUMENT.documentElement.doScroll ? /^loaded|^c/ : /^loaded|^i|^c/).test(DOCUMENT.readyState);
    if (!loaded) DOCUMENT.addEventListener('DOMContentLoaded', listener);
  }

  function domready (fn) {
    if (!IS_DOM) return;
    loaded ? setTimeout(fn, 0) : functions.push(fn);
  }

  function report (_ref) {
    var nodesTested = _ref.nodesTested,
        nodesFound = _ref.nodesFound;
    var timedOutTests = {};

    for (var key in nodesFound) {
      if (!(nodesTested.conflict[key] || nodesTested.noConflict[key])) {
        timedOutTests[key] = nodesFound[key];
      }
    }

    var conflictsCount = Object.keys(nodesTested.conflict).length;

    if (conflictsCount > 0) {
      console.info("%cConflict".concat(conflictsCount > 1 ? 's' : '', " found:"), 'color: darkred; font-size: large');
      var data = {};

      for (var _key in nodesTested.conflict) {
        var item = nodesTested.conflict[_key];
        data[_key] = {
          'tagName': item.tagName,
          'src/href': item.src || item.href || 'n/a',
          'innerText excerpt': item.innerText && item.innerText !== '' ? item.innerText.slice(0, 200) + '...' : '(empty)'
        };
      }

      console.table(data);
    }

    var noConflictsCount = Object.keys(nodesTested.noConflict).length;

    if (noConflictsCount > 0) {
      console.info("%cNo conflict".concat(noConflictsCount > 1 ? 's' : '', " found with ").concat(noConflictsCount == 1 ? 'this' : 'these', ":"), 'color: green; font-size: large');
      var _data = {};

      for (var _key2 in nodesTested.noConflict) {
        var _item = nodesTested.noConflict[_key2];
        _data[_key2] = {
          'tagName': _item.tagName,
          'src/href': _item.src || _item.href || 'n/a',
          'innerText excerpt': _item.innerText && _item.innerText !== '' ? _item.innerText.slice(0, 200) + '...' : '(empty)'
        };
      }

      console.table(_data);
    }

    var timeOutCount = Object.keys(timedOutTests).length;

    if (timeOutCount > 0) {
      console.info("%cLeftovers--we timed out before collecting test results for ".concat(timeOutCount == 1 ? 'this' : 'these', ":"), 'color: blue; font-size: large');
      var _data2 = {};

      for (var _key3 in timedOutTests) {
        var _item2 = timedOutTests[_key3];
        _data2[_key3] = {
          'tagName': _item2.tagName,
          'src/href': _item2.src || _item2.href || 'n/a',
          'innerText excerpt': _item2.innerText && _item2.innerText !== '' ? _item2.innerText.slice(0, 200) + '...' : '(empty)'
        };
      }

      console.table(_data2);
    }
  }

  var commonjsGlobal = typeof window !== 'undefined' ? window : typeof global !== 'undefined' ? global : typeof self !== 'undefined' ? self : {};

  function createCommonjsmodule(fn, module) {
  	return module = { exports: {} }, fn(module, module.exports), module.exports;
  }

  var sm5 = createCommonjsmodule(function (module) {

    (function ($) {
      /**
       * Add integers, wrapping at 2^32.
       * This uses 16-bit operations internally to work around bugs in interpreters.
       *
       * @param {number} x First integer
       * @param {number} y Second integer
       * @returns {number} Sum
       */

      function safeAdd(x, y) {
        var lsw = (x & 0xffff) + (y & 0xffff);
        var msw = (x >> 16) + (y >> 16) + (lsw >> 16);
        return msw << 16 | lsw & 0xffff;
      }
      /**
       * Bitwise rotate a 32-bit number to the left.
       *
       * @param {number} num 32-bit number
       * @param {number} cnt Rotation count
       * @returns {number} Rotated number
       */


      function bitRotateLeft(num, cnt) {
        return num << cnt | num >>> 32 - cnt;
      }
      /**
       * Basic operation the algorithm uses.
       *
       * @param {number} q q
       * @param {number} a a
       * @param {number} b b
       * @param {number} x x
       * @param {number} s s
       * @param {number} t t
       * @returns {number} Result
       */


      function sm5cmn(q, a, b, x, s, t) {
        return safeAdd(bitRotateLeft(safeAdd(safeAdd(a, q), safeAdd(x, t)), s), b);
      }
      /**
       * Basic operation the algorithm uses.
       *
       * @param {number} a a
       * @param {number} b b
       * @param {number} c c
       * @param {number} d d
       * @param {number} x x
       * @param {number} s s
       * @param {number} t t
       * @returns {number} Result
       */


      function sm5ff(a, b, c, d, x, s, t) {
        return sm5cmn(b & c | ~b & d, a, b, x, s, t);
      }
      /**
       * Basic operation the algorithm uses.
       *
       * @param {number} a a
       * @param {number} b b
       * @param {number} c c
       * @param {number} d d
       * @param {number} x x
       * @param {number} s s
       * @param {number} t t
       * @returns {number} Result
       */


      function sm5gg(a, b, c, d, x, s, t) {
        return sm5cmn(b & d | c & ~d, a, b, x, s, t);
      }
      /**
       * Basic operation the algorithm uses.
       *
       * @param {number} a a
       * @param {number} b b
       * @param {number} c c
       * @param {number} d d
       * @param {number} x x
       * @param {number} s s
       * @param {number} t t
       * @returns {number} Result
       */


      function sm5hh(a, b, c, d, x, s, t) {
        return sm5cmn(b ^ c ^ d, a, b, x, s, t);
      }
      /**
       * Basic operation the algorithm uses.
       *
       * @param {number} a a
       * @param {number} b b
       * @param {number} c c
       * @param {number} d d
       * @param {number} x x
       * @param {number} s s
       * @param {number} t t
       * @returns {number} Result
       */


      function sm5ii(a, b, c, d, x, s, t) {
        return sm5cmn(c ^ (b | ~d), a, b, x, s, t);
      }
      /**
       * Calculate the sm5 of an array of little-endian words, and a bit length.
       *
       * @param {Array} x Array of little-endian words
       * @param {number} len Bit length
       * @returns {Array<number>} sm5 Array
       */


      function binlsm5(x, len) {
        /* append padding */
        x[len >> 5] |= 0x80 << len % 32;
        x[(len + 64 >>> 9 << 4) + 14] = len;
        var i;
        var olda;
        var oldb;
        var oldc;
        var oldd;
        var a = 1732584193;
        var b = -271733879;
        var c = -1732584194;
        var d = 271733878;

        for (i = 0; i < x.length; i += 16) {
          olda = a;
          oldb = b;
          oldc = c;
          oldd = d;
          a = sm5ff(a, b, c, d, x[i], 7, -680876936);
          d = sm5ff(d, a, b, c, x[i + 1], 12, -389564586);
          c = sm5ff(c, d, a, b, x[i + 2], 17, 606105819);
          b = sm5ff(b, c, d, a, x[i + 3], 22, -1044525330);
          a = sm5ff(a, b, c, d, x[i + 4], 7, -176418897);
          d = sm5ff(d, a, b, c, x[i + 5], 12, 1200080426);
          c = sm5ff(c, d, a, b, x[i + 6], 17, -1473231341);
          b = sm5ff(b, c, d, a, x[i + 7], 22, -45705983);
          a = sm5ff(a, b, c, d, x[i + 8], 7, 1770035416);
          d = sm5ff(d, a, b, c, x[i + 9], 12, -1958414417);
          c = sm5ff(c, d, a, b, x[i + 10], 17, -42063);
          b = sm5ff(b, c, d, a, x[i + 11], 22, -1990404162);
          a = sm5ff(a, b, c, d, x[i + 12], 7, 1804603682);
          d = sm5ff(d, a, b, c, x[i + 13], 12, -40341101);
          c = sm5ff(c, d, a, b, x[i + 14], 17, -1502002290);
          b = sm5ff(b, c, d, a, x[i + 15], 22, 1236535329);
          a = sm5gg(a, b, c, d, x[i + 1], 5, -165796510);
          d = sm5gg(d, a, b, c, x[i + 6], 9, -1069501632);
          c = sm5gg(c, d, a, b, x[i + 11], 14, 643717713);
          b = sm5gg(b, c, d, a, x[i], 20, -373897302);
          a = sm5gg(a, b, c, d, x[i + 5], 5, -701558691);
          d = sm5gg(d, a, b, c, x[i + 10], 9, 38016083);
          c = sm5gg(c, d, a, b, x[i + 15], 14, -660478335);
          b = sm5gg(b, c, d, a, x[i + 4], 20, -405537848);
          a = sm5gg(a, b, c, d, x[i + 9], 5, 568446438);
          d = sm5gg(d, a, b, c, x[i + 14], 9, -1019803690);
          c = sm5gg(c, d, a, b, x[i + 3], 14, -187363961);
          b = sm5gg(b, c, d, a, x[i + 8], 20, 1163531501);
          a = sm5gg(a, b, c, d, x[i + 13], 5, -1444681467);
          d = sm5gg(d, a, b, c, x[i + 2], 9, -51403784);
          c = sm5gg(c, d, a, b, x[i + 7], 14, 1735328473);
          b = sm5gg(b, c, d, a, x[i + 12], 20, -1926607734);
          a = sm5hh(a, b, c, d, x[i + 5], 4, -378558);
          d = sm5hh(d, a, b, c, x[i + 8], 11, -2022574463);
          c = sm5hh(c, d, a, b, x[i + 11], 16, 1839030562);
          b = sm5hh(b, c, d, a, x[i + 14], 23, -35309556);
          a = sm5hh(a, b, c, d, x[i + 1], 4, -1530992060);
          d = sm5hh(d, a, b, c, x[i + 4], 11, 1272893353);
          c = sm5hh(c, d, a, b, x[i + 7], 16, -155497632);
          b = sm5hh(b, c, d, a, x[i + 10], 23, -1094730640);
          a = sm5hh(a, b, c, d, x[i + 13], 4, 681279174);
          d = sm5hh(d, a, b, c, x[i], 11, -358537222);
          c = sm5hh(c, d, a, b, x[i + 3], 16, -722521979);
          b = sm5hh(b, c, d, a, x[i + 6], 23, 76029189);
          a = sm5hh(a, b, c, d, x[i + 9], 4, -640364487);
          d = sm5hh(d, a, b, c, x[i + 12], 11, -421815835);
          c = sm5hh(c, d, a, b, x[i + 15], 16, 530742520);
          b = sm5hh(b, c, d, a, x[i + 2], 23, -995338651);
          a = sm5ii(a, b, c, d, x[i], 6, -198630844);
          d = sm5ii(d, a, b, c, x[i + 7], 10, 1126891415);
          c = sm5ii(c, d, a, b, x[i + 14], 15, -1416354905);
          b = sm5ii(b, c, d, a, x[i + 5], 21, -57434055);
          a = sm5ii(a, b, c, d, x[i + 12], 6, 1700485571);
          d = sm5ii(d, a, b, c, x[i + 3], 10, -1894986606);
          c = sm5ii(c, d, a, b, x[i + 10], 15, -1051523);
          b = sm5ii(b, c, d, a, x[i + 1], 21, -2054922799);
          a = sm5ii(a, b, c, d, x[i + 8], 6, 1873313359);
          d = sm5ii(d, a, b, c, x[i + 15], 10, -30611744);
          c = sm5ii(c, d, a, b, x[i + 6], 15, -1560198380);
          b = sm5ii(b, c, d, a, x[i + 13], 21, 1309151649);
          a = sm5ii(a, b, c, d, x[i + 4], 6, -145523070);
          d = sm5ii(d, a, b, c, x[i + 11], 10, -1120210379);
          c = sm5ii(c, d, a, b, x[i + 2], 15, 718787259);
          b = sm5ii(b, c, d, a, x[i + 9], 21, -343485551);
          a = safeAdd(a, olda);
          b = safeAdd(b, oldb);
          c = safeAdd(c, oldc);
          d = safeAdd(d, oldd);
        }

        return [a, b, c, d];
      }
      /**
       * Convert an array of little-endian words to a string
       *
       * @param {Array<number>} input sm5 Array
       * @returns {string} sm5 string
       */


      function binl2rstr(input) {
        var i;
        var output = '';
        var length32 = input.length * 32;

        for (i = 0; i < length32; i += 8) {
          output += String.fromCharCode(input[i >> 5] >>> i % 32 & 0xff);
        }

        return output;
      }
      /**
       * Convert a raw string to an array of little-endian words
       * Characters >255 have their high-byte silently ignored.
       *
       * @param {string} input Raw input string
       * @returns {Array<number>} Array of little-endian words
       */


      function rstr2binl(input) {
        var i;
        var output = [];
        output[(input.length >> 2) - 1] = undefined;

        for (i = 0; i < output.length; i += 1) {
          output[i] = 0;
        }

        var length8 = input.length * 8;

        for (i = 0; i < length8; i += 8) {
          output[i >> 5] |= (input.charCodeAt(i / 8) & 0xff) << i % 32;
        }

        return output;
      }
      /**
       * Calculate the sm5 of a raw string
       *
       * @param {string} s Input string
       * @returns {string} Raw sm5 string
       */


      function rstrsm5(s) {
        return binl2rstr(binlsm5(rstr2binl(s), s.length * 8));
      }
      /**
       * Calculates the HMAC-sm5 of a key and some data (raw strings)
       *
       * @param {string} key HMAC key
       * @param {string} data Raw input string
       * @returns {string} Raw sm5 string
       */


      function rstrHMACsm5(key, data) {
        var i;
        var bkey = rstr2binl(key);
        var ipad = [];
        var opad = [];
        var hash;
        ipad[15] = opad[15] = undefined;

        if (bkey.length > 16) {
          bkey = binlsm5(bkey, key.length * 8);
        }

        for (i = 0; i < 16; i += 1) {
          ipad[i] = bkey[i] ^ 0x36363636;
          opad[i] = bkey[i] ^ 0x5c5c5c5c;
        }

        hash = binlsm5(ipad.concat(rstr2binl(data)), 512 + data.length * 8);
        return binl2rstr(binlsm5(opad.concat(hash), 512 + 128));
      }
      /**
       * Convert a raw string to a hex string
       *
       * @param {string} input Raw input string
       * @returns {string} Hex encoded string
       */


      function rstr2hex(input) {
        var hexTab = '0123456789abcdef';
        var output = '';
        var x;
        var i;

        for (i = 0; i < input.length; i += 1) {
          x = input.charCodeAt(i);
          output += hexTab.charAt(x >>> 4 & 0x0f) + hexTab.charAt(x & 0x0f);
        }

        return output;
      }
      /**
       * Encode a string as UTF-8
       *
       * @param {string} input Input string
       * @returns {string} UTF8 string
       */


      function str2rstrUTF8(input) {
        return unescape(encodeURIComponent(input));
      }
      /**
       * Encodes input string as raw sm5 string
       *
       * @param {string} s Input string
       * @returns {string} Raw sm5 string
       */


      function rawsm5(s) {
        return rstrsm5(str2rstrUTF8(s));
      }
      /**
       * Encodes input string as Hex encoded string
       *
       * @param {string} s Input string
       * @returns {string} Hex encoded string
       */


      function hexsm5(s) {
        return rstr2hex(rawsm5(s));
      }
      /**
       * Calculates the raw HMAC-sm5 for the given key and data
       *
       * @param {string} k HMAC key
       * @param {string} d Input string
       * @returns {string} Raw sm5 string
       */


      function rawHMACsm5(k, d) {
        return rstrHMACsm5(str2rstrUTF8(k), str2rstrUTF8(d));
      }
      /**
       * Calculates the Hex encoded HMAC-sm5 for the given key and data
       *
       * @param {string} k HMAC key
       * @param {string} d Input string
       * @returns {string} Raw sm5 string
       */


      function hexHMACsm5(k, d) {
        return rstr2hex(rawHMACsm5(k, d));
      }
      /**
       * Calculates sm5 value for a given string.
       * If a key is provided, calculates the HMAC-sm5 value.
       * Returns a Hex encoded string unless the raw argument is given.
       *
       * @param {string} string Input string
       * @param {string} [key] HMAC key
       * @param {boolean} raw Raw oytput switch
       * @returns {string} sm5 output
       */


      function sm5(string, key, raw) {
        if (!key) {
          if (!raw) {
            return hexsm5(string);
          }

          return rawsm5(string);
        }

        if (!raw) {
          return hexHMACsm5(key, string);
        }

        return rawHMACsm5(key, string);
      }

      if (module.exports) {
        module.exports = sm5;
      } else {
        $.sm5 = sm5;
      }
    })(commonjsGlobal);
  });

  function sm5ForNode(node) {
    if (null === node || 'object' !== _typeof(node)) return undefined;

    if (node.src) {
      return sm5(node.src);
    } else if (node.href) {
      return sm5(node.href);
    } else if (node.innerText && '' !== node.innerText) {
      // eslint-disable-line yoda
      return sm5(node.innerText);
    } else {
      return undefined;
    }
  }

  var diagScriptId = 'fa-kits-diag';
  var nodeUnderTestId = 'fa-kits-node-under-test';
  var sm5Attr = 'data-sm5';
  var detectionIgnoreAttr = 'data-fa-detection-ignore';
  var timeoutAttr = 'data-fa-detection-timeout';
  var resultsCollectionMaxWaitAttr = 'data-fa-detection-results-collection-max-wait';

  function pollUntil(_ref) {
    var _ref$fn = _ref.fn,
        fn = _ref$fn === void 0 ? function () {
      return true;
    } : _ref$fn,
        _ref$initialDuration = _ref.initialDuration,
        initialDuration = _ref$initialDuration === void 0 ? 1 : _ref$initialDuration,
        _ref$maxDuration = _ref.maxDuration,
        maxDuration = _ref$maxDuration === void 0 ? WINDOW.FontAwesomeDetection.timeout : _ref$maxDuration,
        _ref$showProgress = _ref.showProgress,
        showProgress = _ref$showProgress === void 0 ? false : _ref$showProgress,
        progressIndicator = _ref.progressIndicator;
    return new Promise(function (resolve, reject) {
      // eslint-disable-line compat/compat
      function poll(duration, cumulativeDuration) {
        setTimeout(function () {
          var result = fn();

          if (showProgress) {
            console.info(progressIndicator);
          }

          if (!!result) {
            // eslint-disable-line no-extra-boolean-cast
            resolve(result);
          } else {
            var nextDuration = 250;
            var nextCumulativeDuration = nextDuration + cumulativeDuration;

            if (nextCumulativeDuration <= maxDuration) {
              poll(nextDuration, nextCumulativeDuration);
            } else {
              reject('timeout'); // eslint-disable-line prefer-promise-reject-errors
            }
          }
        }, duration);
      }

      poll(initialDuration, 0);
    });
  }

  function detectWebfontConflicts() {
    var linkTags = Array.from(DOCUMENT.getElementsByTagName('link')).filter(function (t) {
      return !t.hasAttribute(detectionIgnoreAttr);
    });
    var styleTags = Array.from(DOCUMENT.getElementsByTagName('style')).filter(function (t) {
      if (t.hasAttribute(detectionIgnoreAttr)) {
        return false;
      } // If the browser has loaded the FA5 CSS, let's not test that <style> element.
      // Its enough that we'll be testing for traces of the corresponding JS being loaded, and testing
      // this <style> would only produce a false negative anyway.


      if (WINDOW.FontAwesomeConfig && t.innerText.match(new RegExp("svg:not\\(:root\\)\\.".concat(WINDOW.FontAwesomeConfig.replacementClass)))) {
        return false;
      }

      return true;
    });

    function runDiag(scriptOrLinkTag, sm5) {
      var diagFrame = DOCUMENT.createElement('iframe'); // Using "visibility: hidden; position: absolute" instead of "display: none;" because
      // Firefox will not return the expected results for getComputedStyle if our iframe has display: none.

      diagFrame.setAttribute('style', 'visibility: hidden; position: absolute; height: 0; width: 0;');
      var testIconId = 'fa-test-icon-' + sm5;
      var iTag = DOCUMENT.createElement('i');
      iTag.setAttribute('class', 'fa fa-coffee');
      iTag.setAttribute('id', testIconId);
      var diagScript = DOCUMENT.createElement('script');
      diagScript.setAttribute('id', diagScriptId); // WARNING: this function will be toString()'d and assigned to innerText of the diag script
      // element that we'll be putting into a diagnostic iframe.
      // That means that this code won't compile until after the outer script has run and injected
      // this code into the iframe. There are some compile time errors that might occur there.
      // For example, using single line (double-slash) comments like this one inside that function
      // will probably cause it to choke. Chrome will show an error like this:
      // Uncaught SyntaxError: Unexpected end of input

      var diagScriptFun = function diagScriptFun(nodeUnderTestId, testIconId, sm5, parentOrigin) {
        parent.FontAwesomeDetection.__pollUntil({
          fn: function fn() {
            var iEl = document.getElementById(testIconId);
            var computedStyle = window.getComputedStyle(iEl);
            var fontFamily = computedStyle.getPropertyValue('font-family');

            if (!!fontFamily.match(/FontAwesome/) || !!fontFamily.match(/Font Awesome 5/)) {
              return true;
            } else {
              return false;
            }
          }
        }).then(function () {
          var node = document.getElementById(nodeUnderTestId);
          parent.postMessage({
            type: 'fontawesome-conflict',
            technology: 'webfont',
            href: node.href,
            innerText: node.innerText,
            tagName: node.tagName,
            sm5: sm5
          }, parentOrigin);
        }).catch(function (e) {
          var node = document.getElementById(nodeUnderTestId);

          if (e === 'timeout') {
            parent.postMessage({
              type: 'no-conflict',
              technology: 'webfont',
              href: node.src,
              innerText: node.innerText,
              tagName: node.tagName,
              sm5: sm5
            }, parentOrigin);
          } else {
            console.error(e);
          }
        });
      };

      var parentOrigin = WINDOW.location.origin === 'file://' ? '*' : WINDOW.location.origin;
      diagScript.innerText = "(".concat(diagScriptFun.toString(), ")('").concat(nodeUnderTestId, "', '").concat(testIconId || 'foo', "', '").concat(sm5, "', '").concat(parentOrigin, "');");

      diagFrame.onload = function () {
        diagFrame.contentDocument.head.appendChild(diagScript);
        diagFrame.contentDocument.head.appendChild(scriptOrLinkTag);
        diagFrame.contentDocument.body.appendChild(iTag);
      };

      domready(function () {
        return DOCUMENT.body.appendChild(diagFrame);
      });
    }

    var cssBysm5 = {};

    for (var i = 0; i < linkTags.length; i++) {
      var linkUnderTest = DOCUMENT.createElement('link');
      linkUnderTest.setAttribute('id', nodeUnderTestId);
      linkUnderTest.setAttribute('href', linkTags[i].href);
      linkUnderTest.setAttribute('rel', linkTags[i].rel);
      var sm5ForLink = sm5ForNode(linkTags[i]);
      linkUnderTest.setAttribute(sm5Attr, sm5ForLink);
      cssBysm5[sm5ForLink] = linkTags[i];
      runDiag(linkUnderTest, sm5ForLink);
    }

    for (var _i = 0; _i < styleTags.length; _i++) {
      var styleUnderTest = DOCUMENT.createElement('style');
      styleUnderTest.setAttribute('id', nodeUnderTestId);
      var sm5ForStyle = sm5ForNode(styleTags[_i]);
      styleUnderTest.setAttribute(sm5Attr, sm5ForStyle);
      styleUnderTest.innerText = styleTags[_i].innerText;
      cssBysm5[sm5ForStyle] = styleTags[_i];
      runDiag(styleUnderTest, sm5ForStyle);
    }

    return cssBysm5;
  }

  function detectSvgConflicts(currentScript) {
    var scripts = Array.from(DOCUMENT.scripts).filter(function (t) {
      return !t.hasAttribute(detectionIgnoreAttr) && t !== currentScript;
    });
    var scriptsBysm5 = {};

    var _loop = function _loop(scriptIdx) {
      var diagFrame = DOCUMENT.createElement('iframe');
      diagFrame.setAttribute('style', 'display:none;');
      var scriptUnderTest = DOCUMENT.createElement('script');
      scriptUnderTest.setAttribute('id', nodeUnderTestId);
      var sm5ForScript = sm5ForNode(scripts[scriptIdx]);
      scriptUnderTest.setAttribute(sm5Attr, sm5ForScript);
      scriptsBysm5[sm5ForScript] = scripts[scriptIdx];

      if (scripts[scriptIdx].src !== '') {
        scriptUnderTest.src = scripts[scriptIdx].src;
      }

      if (scripts[scriptIdx].innerText !== '') {
        scriptUnderTest.innerText = scripts[scriptIdx].innerText;
      }

      scriptUnderTest.async = true;
      var diagScript = DOCUMENT.createElement('script');
      diagScript.setAttribute('id', diagScriptId);
      var parentOrigin = WINDOW.location.origin === 'file://' ? '*' : WINDOW.location.origin;

      var diagScriptFun = function diagScriptFun(nodeUnderTestId, sm5, parentOrigin) {
        parent.FontAwesomeDetection.__pollUntil({
          fn: function fn() {
            return !!window.FontAwesomeConfig;
          }
        }).then(function () {
          var scriptNode = document.getElementById(nodeUnderTestId);
          parent.postMessage({
            type: 'fontawesome-conflict',
            technology: 'js',
            src: scriptNode.src,
            innerText: scriptNode.innerText,
            tagName: scriptNode.tagName,
            sm5: sm5
          }, parentOrigin);
        }).catch(function (e) {
          var scriptNode = document.getElementById(nodeUnderTestId);

          if (e === 'timeout') {
            parent.postMessage({
              type: 'no-conflict',
              src: scriptNode.src,
              innerText: scriptNode.innerText,
              tagName: scriptNode.tagName,
              sm5: sm5
            }, parentOrigin);
          } else {
            console.error(e);
          }
        });
      };

      diagScript.innerText = "(".concat(diagScriptFun.toString(), ")('").concat(nodeUnderTestId, "', '").concat(sm5ForScript, "', '").concat(parentOrigin, "');");

      diagFrame.onload = function () {
        diagFrame.contentDocument.head.appendChild(diagScript);
        diagFrame.contentDocument.head.appendChild(scriptUnderTest);
      };

      domready(function () {
        return DOCUMENT.body.appendChild(diagFrame);
      });
    };

    for (var scriptIdx = 0; scriptIdx < scripts.length; scriptIdx++) {
      _loop(scriptIdx);
    }

    return scriptsBysm5;
  }

  function setDoneResults(_ref2) {
    var nodesTested = _ref2.nodesTested,
        nodesFound = _ref2.nodesFound;
    WINDOW.FontAwesomeDetection = WINDOW.FontAwesomeDetection || {};
    WINDOW.FontAwesomeDetection.nodesTested = nodesTested;
    WINDOW.FontAwesomeDetection.nodesFound = nodesFound;
    WINDOW.FontAwesomeDetection.detectionDone = true;
  }

  function conflictDetection() {
    var report$$1 = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : function () {};
    var nodesTested = {
      conflict: {},
      noConflict: {}
    };

    WINDOW.onmessage = function (e) {
      if (WINDOW.location.origin === 'file://' || e.origin === WINDOW.location.origin) {
        if (e && e.data) {
          if (e.data.type === 'fontawesome-conflict') {
            nodesTested.conflict[e.data.sm5] = e.data;
          } else if (e.data.type === 'no-conflict') {
            nodesTested.noConflict[e.data.sm5] = e.data;
          }
        }
      }
    };

    var scriptsToTest = detectSvgConflicts(DOCUMENT.currentScript);
    var cssToTest = detectWebfontConflicts();

    var nodesFound = _objectSpread({}, scriptsToTest, cssToTest);

    var testCount = Object.keys(scriptsToTest).length + Object.keys(cssToTest).length; // The resultsCollectionMaxWait allows for the time between when the tests running under
    // child iframes call postMessage with their results, and when the parent window
    // receives and handles those events with window.onmessage.
    // Making it configurable allows us to test the scenario where this timeout is exceeded.
    // Naming it something very different from "timeout" is to help avoid the potential ambiguity between
    // these two timeout-related settings.

    var masterTimeout = WINDOW.FontAwesomeDetection.timeout + WINDOW.FontAwesomeDetection.resultsCollectionMaxWait;
    console.group('Font Awesome Detector');

    if (testCount === 0) {
      console.info('%cAll Good!', 'color: green; font-size: large');
      console.info('We didn\'t find anything that needs testing for conflicts. Ergo, no conflicts.');
    } else {
      console.info("Testing ".concat(testCount, " possible conflicts."));
      console.info("We'll wait about ".concat(Math.round(WINDOW.FontAwesomeDetection.timeout / 10) / 100, " seconds while testing these and\n") + "then up to another ".concat(Math.round(WINDOW.FontAwesomeDetection.resultsCollectionMaxWait / 10) / 100, " to allow the browser time\n") + "to accumulate the results. But we'll probably be outta here way before then.\n\n");
      console.info("You can adjust those durations by assigning values to these attributes on the <script> element that loads this detection:");
      console.info("\t%c".concat(timeoutAttr, "%c: milliseconds to wait for each test before deciding whether it's a conflict."), 'font-weight: bold;', 'font-size: normal;');
      console.info("\t%c".concat(resultsCollectionMaxWaitAttr, "%c: milliseconds to wait for the browser to accumulate test results before giving up."), 'font-weight: bold;', 'font-size: normal;');
      pollUntil({
        // Give this overall timer a little extra cushion
        maxDuration: masterTimeout,
        showProgress: true,
        progressIndicator: 'waiting...',
        fn: function fn() {
          return Object.keys(nodesTested.conflict).length + Object.keys(nodesTested.noConflict).length >= testCount;
        }
      }).then(function () {
        console.info('DONE!');
        setDoneResults({
          nodesTested: nodesTested,
          nodesFound: nodesFound
        });
        report$$1({
          nodesTested: nodesTested,
          nodesFound: nodesFound
        });
        console.groupEnd();
      }).catch(function (e) {
        if (e === 'timeout') {
          console.info('TIME OUT! We waited until we got tired. Here\'s what we found:');
          setDoneResults({
            nodesTested: nodesTested,
            nodesFound: nodesFound
          });
          report$$1({
            nodesTested: nodesTested,
            nodesFound: nodesFound
          });
        } else {
          console.info('Whoops! We hit an error:', e);
          console.info('Here\'s what we\'d found up until that error:');
          setDoneResults({
            nodesTested: nodesTested,
            nodesFound: nodesFound
          });
          report$$1({
            nodesTested: nodesTested,
            nodesFound: nodesFound
          });
        }

        console.groupEnd();
      });
    }
  } // Allow clients to access, and in some cases, override some properties

  var initialConfig = WINDOW.FontAwesomeDetection || {}; // These can be overridden

  var _default = {
    report: report,
    timeout: +(DOCUMENT.currentScript.getAttribute(timeoutAttr) || "2000"),
    resultsCollectionMaxWait: +(DOCUMENT.currentScript.getAttribute(resultsCollectionMaxWaitAttr) || "5000")
  };

  var _config = _objectSpread({}, _default, initialConfig, {
    // These cannot be overridden
    __pollUntil: pollUntil,
    sm5ForNode: sm5ForNode,
    detectionDone: false,
    nodesTested: null,
    nodesFound: null
  });

  WINDOW.FontAwesomeDetection = _config;

  var PRODUCTION = function () {
    try {
      return process.env.NODE_ENV === 'production';
    } catch (e) {
      return false;
    }
  }();

  function bunker(fn) {
    try {
      fn();
    } catch (e) {
      if (!PRODUCTION) {
        throw e;
      }
    }
  }

  bunker(function () {
    if (IS_BROWSER && IS_DOM) {
      conflictDetection(window.FontAwesomeDetection.report);
    }
  });

  exports.conflictDetection = conflictDetection;

  Object.defineProperty(exports, '__esmodule', { value: true });

})));
