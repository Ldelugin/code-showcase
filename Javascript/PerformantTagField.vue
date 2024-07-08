<template>

    <b-form-group
        v-bind="formGroupProps"
        :class="{'mb-0': noMargin}"
    >
        <b-input-group :size="size">
            <b-form-textarea
                :id="id"
                v-model="text"
                :size="size"
                rows="1"
                :placeholder="placeholder"
                no-resize
            >
            </b-form-textarea>
            
            <template #prepend>
                <b-input-group-text>{{ formatWithCommas(value.length) }}</b-input-group-text>
            </template>
            
            <b-input-group-append>
                <b-button
                    :size="size"
                    @click="addNewTags"
                    :disabled="text === ''"
                >
                    <font-awesome-icon icon="plus"></font-awesome-icon>
                    Add
                </b-button>
                <b-button
                    :size="size"
                    @click="clearAll"
                    :disabled="value.length === 0"
                    v-if="!hideClearButton"
                >
                    <font-awesome-icon icon="eraser"></font-awesome-icon>
                    Clear all
                </b-button>
            </b-input-group-append>
        </b-input-group>
    </b-form-group>

</template>

<script>
    import guidsMixin from "../Mixins/guidsMixin"
    import formatMixin from "../Mixins/formatMixin"

    export default {
        name: "PerformantTagField",
        mixins: [guidsMixin, formatMixin],
        props: {
            value: {
                type: Array,
                required: true
            },
            limit: {
                type: Number,
                default: 5
            },
            placeholder: {
                type: String
            },
            size: {
                type: String,
                default: 'sm'
            },
            id: {
                type: String,
                required: true
            },
            label: {
                type: String
            },
            noMargin: {
                type: Boolean
            },
            hideClearButton: {
                type: Boolean
            },
            supportsOnlyGuids: {
                type: Boolean
            }
        },
        data: () => ({
            text: ''
        }),
        watch: {
            value() {
                if(this.value.length === 0)
                {
                    this.text = '';
                }
            }
        },
        computed: {
            formGroupProps() {
                let props = {'label-for': this.id};

                if (!this.label || this.label.length < 1) {
                    return props;
                }

                props['label'] = this.label;
                return props;
            }
        },
        methods: {
            addNewTags() {
                let allTags = new Set(this.value);
                for (let tag of this.text.split(/[\s,;]/)) {
                    if (tag) {
                        if (this.supportsOnlyGuids && !this.isValidGuid(tag)) {
                            continue;
                        }

                        allTags.add(tag);
                    }
                }
                this.$emit('input', [...allTags]);
            },
            /**
             * Clear all the tags and text.
             */
            clearAll() {
                this.text = '';
                this.$emit('input', []);
            }
        }
    }
</script>