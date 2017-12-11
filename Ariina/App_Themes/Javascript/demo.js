'use strict';

// Strict Mode is a new feature in ECMAScript 5 that allows
// you to place a program, or a function, in a "strict" operating
// context. This strict context prevents certain actions from
// being taken and throws more exceptions
// 1. It cathches some common coding bloopers, throwing exceptions
// 2. It prevents, or throws errors, when relatively "unsafe" actions
//    are taken (such as gaining access to the global object).
// 3. It disables features that are confusing or poorly thought out

(function() {
    var amount = document.querySelector('#amount');
    var amountLabel = document.querySelector('label[for="amount"]');

    amount.addEventListener('focus',
        function() {
            amountLabel.className = 'has-focus';
        },
        false);
    amount.addEventListener('blur',
        function() {
            amountLabel.className = '';
        },
        false);
})();