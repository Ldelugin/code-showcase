<template>
    <b-form-group
        :label="label"
        :label-for="id"
        :description="description"
        :class="{'mb-0': noMargin, 'px-0': noPadding}"
    >
        <b-form-tags
            :id="id"
            v-model="selectedOptions"
            @input="onSelectedOptionsChanged"
            no-outer-focus
            :class="{'mb-0': noMargin, 'mb-2': !noMargin, 'px-0': noPadding, 'px-2': !noPadding, 'border-0': true}"
        >
            <template v-slot="{ tags, disabled, addTag, removeTag }">
                <b-dropdown
                    v-if="!readOnly"
                    :size="size"
                    variant="outline-secondary"
                    block
                    menu-class="w-100"
                    toggle-class="text-left"
                    no-caret
                >
                    <template #button-content>
                      {{ name }}
                      <div class="text-right float-right">
                        <font-awesome-icon icon="caret-down"></font-awesome-icon>
                      </div>
                    </template>
                    <b-dropdown-form @submit.stop.prevent="() => {}">
                        <b-form-group
                            :description="searchDesc"
                            :disabled="disabled"
                            :label-for="'tag-search-input'+id"
                            class="mb-0"
                            label="Search:"
                            label-cols-md="auto"
                            :label-size="size"
                        >
                            <b-form-input
                                :id="'tag-search-input'+id"
                                v-model="search"
                                autocomplete="off"
                                :size="size"
                                type="search"
                            ></b-form-input>
                        </b-form-group>
                    </b-dropdown-form>
                    <b-dropdown-divider></b-dropdown-divider>
                    <b-dropdown-item-button
                        v-for="option in availableOptions"
                        :key="option"
                        @click="onOptionClick({ option, addTag })"
                    >
                        {{ option }}
                    </b-dropdown-item-button>
                    <b-dropdown-text v-if="availableOptions.length === 0">
                        There is no {{ name }} left to select
                    </b-dropdown-text>
                </b-dropdown>
              
              <ul
                  v-if="tags.length > 0"
                  class="list-inline d-inline-block mb-2"
              >
                <li
                    v-for="tag in tags"
                    :key="tag"
                    class="list-inline-item"
                >
                  <b-form-tag
                      @remove="removeTag(tag)"
                      :title="tag"
                      :disabled="disabled"
                      :variant="tagVariant"
                      :no-remove="readOnly"
                  >{{ tag }}</b-form-tag>
                </li>
              </ul>
            </template>
        </b-form-tags>
    </b-form-group>
</template>

<script>
export default {
    name: "TagBasedSelector",
    props: {
        allOptions: {
            type: Array,
            required: true,
            default: () => []
        },
        name: {
            type: String,
            required: true
        },
        label: {
            type: String
        },
        description: {
            type: String
        },
        id: {
            type: String,
            required: true
        },
        tagVariant: {
            type: String,
            default: 'info'
        },
        readOnly :{
            type: Boolean
        },
        noMargin: {
            type: Boolean
        },
        noPadding: {
            type: Boolean
        },
        value: {
            required: true
        },
        size: {
            type: String,
            default: 'md'
        }
    },
    data: () => ({
        search: '',
        selectedOptions: []
    }),
    computed: {
        criteria() {
            return this.search.trim().toLowerCase();
        },
        availableOptions() {
            const criteria = this.criteria;
            const options = this.allOptions.filter(option => this.selectedOptions.indexOf(option) === -1);
            if (criteria) {
                return options.filter(option => option.toLowerCase().indexOf(criteria) > -1);
            }

            return options;
        },
        searchDesc() {
            if (this.criteria && this.availableOptions.length === 0) {
                return `There are no ${this.name}s matching your search criteria`;
            }

            return '';
        },
        formGroupProps() {
            let props = {'label-for': this.id};

            if (!this.label || this.label.length < 1) {
              props['label'] = this.label;
            }
            
            if (!this.description || this.description.length < 1) {
              props['description'] = this.description;
            }
            return props;
        }
    },
    created() {
        this.selectedOptions = this.value;
    },
    methods: {
        onOptionClick({option, addTag}) {
            addTag(option);
            this.search = '';

        },
        onSelectedOptionsChanged(options) {
            this.$emit('input', options);
        }
    },
    watch: {
        value() {
            this.selectedOptions = this.value;
        }
    }
}
</script>

<style scoped>
    .b-form-tags {
      background: none;
    }
</style>