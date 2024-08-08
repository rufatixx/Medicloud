class PhoneField {
    constructor(a, b = '+994(__) ___-__-__', c = '_') {
        this.handler = a, this.mask = b, this.placeholder = c, this.setLength(), this.setValue(), this.start = this.placeHolderPosition() - 1, this.handler.addEventListener('focusin', () => {
            this.focused()
        }), this.handler.addEventListener('keydown', d => {
            this.input(d)
        })
    }
    focused() {
        let a = this.placeHolderPosition();
        this.handler.selectionStart = a, this.handler.selectionEnd = a
    }
	input(a) {
		if (this.isDirectionKey(a.key) || a.preventDefault()) {
			return;
		}

		if (this.isNum(a.key)) {
			this.changeChar(a.key);
		} else if (this.isDeletionKey(a.key)) {
			let position = this.handler.selectionStart;

			if ('Backspace' === a.key) {
				this.changeChar(this.placeholder, -1, position);
			} else if ('Delete' === a.key) {
				this.changeChar(this.placeholder);
			}
		}
    }
    setLength() {
        this.handler.maxLength = this.mask.length
    }
    setValue() {
        this.handler.value = this.mask
    }
    isNum(a) {
        return !isNaN(a) && parseInt(+a) == a && !isNaN(parseInt(a, 10))
    }
    isDeletionKey(a) {
        return 'Delete' === a || 'Backspace' === a
    }
    isDirectionKey(a) {
        return 'ArrowUp' === a || 'ArrowDown' === a || 'ArrowRight' === a || 'ArrowLeft' === a || 'Tab' === a
    }
    isPlaceholder(a) {
        return a == this.placeholder
    }
    placeHolderPosition() {
        return this.handler.value.indexOf(this.placeholder)
    }
    changeChar(a, b = 1, c = this.mask.length) {
		let d = this.handler.value;
		let f = (0 < b) ? this.handler.selectionStart : this.handler.selectionStart - 1;

		if (f === c) return false;

		if (!this.isNum(d[f]) && !this.isPlaceholder(d[f])) {
			while (f >= 0 && f < c && (!this.isNum(d[f]) && !this.isPlaceholder(d[f]))) {
				f += b;
			}
		}

		if (f >= 0 && f < c) {
			if (this.shouldSkipDeletion(d, f)) {
				return;
			}

			let updatedValue = this.replaceAt(d, f, a);
			this.handler.value = updatedValue;

			f += (0 < b) ? b : 0;
			this.handler.selectionStart = f;
			this.handler.selectionEnd = f;
		}
    }
    replaceAt(a, b, c) {
        return a.substring(0, b) + c + a.substring(++b)
	}
	shouldSkipDeletion(value, index) {
		return value.startsWith('+994', 0) && index < 4;
	}
}
document.addEventListener('DOMContentLoaded', function() {
    'use strict';
    let a = document.getElementsByClassName('masked-phone'),
        b = [];
    for (let c = 0; c < a.length; c++) b.push(new PhoneField(a[c], a[c].dataset.phonemask, a[c].dataset.placeholder))
});