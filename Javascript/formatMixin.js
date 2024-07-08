export default {
    methods: {
        /**
         * Format the given `value` with a comma every 3 decimal places for numbers larger than 999.
         *
         * @param number - The number that should be formatted with commas.
         * @return {string} - A `string` version of the given `number` with a comma every 3 decimal
         * places for numbers larger than 999.
         */
        formatWithCommas(number) {
            return Number(number).toLocaleString('en-GB');
        },
        /**
         * Format the given `value` with a minus in front of it.
         * 
         * @param value - The value.
         * @returns {string} - A `string` with a minus in front of the given `value`.
         */
        formatWithMinus(value) {
            if (value > 0)
                return `- ${this.formatWithCommas(value)}`;

            return this.formatWithCommas(value);
        }        
    }
}