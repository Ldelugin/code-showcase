export default {
    methods: {
        /**
         * Create a csv file from the given data and download it when the content is created.
         * @param data An array with all the items to add as rows in the csv file.
         * @param fileName The name of the csv file.
         * @param allowedKeys An array with all the allowed keys to include.
         * @param header An optional header that is written to the first line of the CSV file.
         * @param batchSize The batchSize.
         */
        csvExport(data, fileName, allowedKeys, header = null, batchSize = 100000) {
            let filterOutKeys = true;
            if (allowedKeys === undefined) {
                allowedKeys = Object.keys(data[0]);
                filterOutKeys = false;
            } else {
                allowedKeys = allowedKeys.map(k => k.key);
            }
            
            let appendCountToFileName = false;
            if (this.isAppendingWithFileCountNeeded(data.length, batchSize) && fileName.endsWith('.csv')) {
                let indexOfPoint = fileName.lastIndexOf('.');
                fileName = fileName.substr(0, indexOfPoint);
                appendCountToFileName  = true;
            }

            let indexes = [];
            let totalCount = data.length;
            let totalFiles = Math.ceil(totalCount / batchSize);
            for (let i = totalFiles - 1; i >= 0; i--) {
                indexes.push({
                    start: i * batchSize,
                    end: (i + 1) * batchSize
                });
            }

            this.createFiles(indexes, appendCountToFileName , fileName, data, allowedKeys, filterOutKeys, header);
        },
        /**
         * Get values from the given object, the returned values depends on the allowedKeys and whether
         * they should be filtered out or not.
         * @param item The object.
         * @param allowedKeys An array with all the allowed keys to include.
         * @param filterOutKeys Whether this function should filter out keys or not.
         * @returns {string} Returns a joined string with all the values separated by a ';'.
         */
        getValues(item, allowedKeys, filterOutKeys) {
            if (!filterOutKeys) {
                return Object.values(item).join(";");
            }
            
            return Object.keys(item)
                .filter(key => allowedKeys.includes(key))
                .map(key => item[key])
                .join(";");
        },
        /**
         * Create the actual csv content and export to the given file name.
         * @param indexes Array with all the indexes needed to slice.
         * @param appendCountToFileName Should the file name be appended with the count.
         * @param fileName The name of the csv file.
         * @param data An array with all the items to add as rows in the csv file.
         * @param allowedKeys An array with all the allowed keys to include.
         * @param filterOutKeys Whether this function should filter out keys or not.
         * @param header An optional header that is written to the first line of the CSV file.
         */
        createFiles(indexes, appendCountToFileName , fileName, data, allowedKeys, filterOutKeys, header) {
            let count = 1;
            let index = indexes.pop();
            
            while(index) {
                let csvContent = "data:text/csv;charset=utf-8,";
                csvContent  += 'sep=;\n';
                if (header) {
                    csvContent += (header + '\n');
                }
                csvContent += [
                    allowedKeys.map(key => key.charAt(0).toUpperCase() + key.slice(1)).join(";"),
                    ...data.slice(index.start, index.end).map(item => this.getValues(item, allowedKeys, filterOutKeys))
                ]
                    .join("\n")
                    .replace(/(^\[)|(]$)/gm, "");

                this.export(csvContent, appendCountToFileName ? this.createFileName(fileName, count) : fileName);
                index = indexes.pop();
                count++;
            }
        },
        /**
         * Export the csv content to the given file name.
         * @param csvContent The csv content.
         * @param fileName The name of the csv file.
         */
        export(csvContent, fileName) {
            const encodedCsvContent = encodeURI(csvContent);
            const link = document.createElement("a");
            link.setAttribute("href", encodedCsvContent);
            link.setAttribute("download", fileName);
            link.setAttribute("onclick", "");
            link.setAttribute("style", "cursor: pointer;");
            link.click();
        },
        /**
         * Append the given file name with the count.
         * @param fileName The file name.
         * @param count The count to append with.
         * @returns {string} The new file name.
         */
        createFileName(fileName, count) {
            return `${fileName}${count}.csv`;
        },
        /**
         * Get a value that indicates whether it is needed to append the file name with the file count.
         * @param total The total amount of rows.
         * @param batchSize The batch size.
         * @returns {boolean} Returns true if the total amount of rows is larger than the batchSize.
         */
        isAppendingWithFileCountNeeded(total, batchSize) {
            return total > batchSize;
        }
    }
}