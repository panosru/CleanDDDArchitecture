'use strict';

import path from 'path';
import fs from 'fs';
import gulp from 'gulp';
import concat from 'gulp-concat';
import deporder from 'gulp-deporder';
import stripdebug from 'gulp-strip-debug';
import uglify from 'gulp-terser';
import autoprefixer from 'autoprefixer';
import postcss from 'gulp-postcss';
import assets from 'postcss-assets';
import cssnano from 'cssnano';
import postcssCopy from 'postcss-copy';
import advancedVariables from 'postcss-advanced-variables';
import postcssUtils from 'postcss-utilities';
import stylelint from 'stylelint';
import postScss from 'postcss-scss';
import precss from 'precss';
import postcssImport from 'postcss-import';
import cssnext from 'postcss-preset-env';
import rucksack from 'rucksack-css';
import fontMagician from 'postcss-font-magician';
import pxToRem from 'postcss-pxtorem';
import Imagemin from 'imagemin';
import imageminPngquant from 'imagemin-pngquant';
import imageminJpegoptim from 'imagemin-jpegoptim';
import imageminGifsicle from 'imagemin-gifsicle';
import babel from 'gulp-babel';
import log from 'gulplog';
import sourcemaps from 'gulp-sourcemaps';
import rename from 'gulp-rename';
import es from 'event-stream';
import resolve from 'resolve';
import gulpif from 'gulp-if';
import del from 'del';
import postcssPurge from '@fullhuman/postcss-purgecss';
import gulpTS from 'gulp-typescript';
import colorguard from 'colorguard';
import cssgrace from 'cssgrace';
import razor from './gulp-razor';
import postcssTransformShortcut from 'postcss-transform-shortcut';

(function () {

    const registerTask = Symbol('registerTask');
    const options = {
        prod: ('prod' === process.env.MIX_ENV),
        app: path.resolve('../Pages'),
        source: path.resolve('../Assets'),
        public: path.resolve('../wwwroot'),
        node_modules: path.resolve('node_modules'),
        bower_components: path.resolve('bower_components')
    };

    function getModule(module, type = 'js') {
        try {
            try {
                return resolve.sync(module);
            } catch (e) {
                return resolve.sync(Builder.paths.bower_components + '/' + module);
            }
        } catch (e) {
            return resolve.sync(Builder.paths[type].source + '/' + module);
        }
    }

    function cssReplace(replacements = []) {

        return (css) => {
            css.walk((node) => {
                switch (node.constructor.name) {
                    case 'Declaration':
                        for (let replacement of replacements) {
                            node.value = node.value.replace(...replacement);
                        }
                        break;
                }
            });
        }
    }

    class Builder {

        static get paths() {
            return {
                app: options.app,
                source: options.source,
                public: options.public,
                node_modules: options.node_modules,
                bower_components: options.bower_components,
                dist: options.public,
                get img() {
                    return {
                        source: this.source + '/images',
                        dist: this.public + '/images'
                    }
                },
                get css() {
                    return {
                        source: this.source + '/css',
                        dist: this.public + '/css'
                    }
                },
                get js() {
                    return {
                        source: this.source + '/js',
                        dist: this.public + '/js'
                    }
                },
                get ts() {
                    return this.js;
                },
                get font() {
                    return {
                        source: this.source + '/fonts',
                        dist: this.public + '/fonts'
                    }
                }
            };
        }

        constructor() {

            let tasks = [
                'omg',
                'clearCache',
                'clean',
                {
                    caller: 'css',
                    deps: {
                        method: 'series',
                        tasks: ['clearCache']
                    }
                },
                {
                    caller: 'js',
                    deps: {
                        method: 'series',
                        tasks: ['clearCache']
                    }
                },
                {
                    caller: 'img',
                    deps: {
                        method: 'series',
                        tasks: ['clearCache']
                    }
                },
            ];

            for (let task of tasks) {
                let params = {};

                if ('object' === typeof task) {

                    if ('undefined' === typeof task.caller) {
                        throw new Error('Task object must have a "caller" property.');
                    }

                    params.task = task.caller;
                    params.deps = task.deps;

                } else if ('string' === typeof task) {
                    params.task = task;
                } else {
                    throw new Error(typeof task + ' type is not allowed as a task.');
                }

                params.cb = this[params.task + 'Task'];

                if ('undefined' === typeof params.cb) {
                    throw new Error('Callback is not defined!');
                }

                this[registerTask](params.task, params.cb, params.deps);
            }

            gulp.task('run', gulp.parallel([
                'clean', 'img', 'css', 'js'
            ]));

            gulp.task('watch', function () {

                // Volt changes
                // gulp.watch(Builder.paths.app + '/katerinakourteli_web/templates/**/*.eex', gulp.parallel(['clearCache']));

                // image changes
                gulp.watch(Builder.paths.img.source + '/**/*', gulp.parallel(['img']));

                // javascript changes
                gulp.watch(Builder.paths.js.source + '/**/*', gulp.parallel(['js']));

                // css changes
                gulp.watch(Builder.paths.css.source + '/**/*', gulp.parallel(['css']));

            });


            gulp.task('default', gulp.series('run', 'watch'));
        }

        [registerTask](task, cb, deps = undefined) {
            if ('undefined' === typeof deps) {
                gulp.task(task, cb);
            } else {
                gulp.task(task, gulp.series(gulp[deps.method](...deps.tasks), cb));
            }
        }

        omgTask(done) {
            console.warn("I'm omg task!");

            return done();
        }

        clearCacheTask(done) {
            console.log("Clearing cache");
            // const dir = path.resolve('cache');
            //
            // fs.readdir(dir, (err, files) => {
            //     if (err) throw err;
            //
            //     for (const file of files) {
            //         fs.unlink(path.join(dir, file), err => {
            //             if (err) throw err;
            //         });
            //     }
            // });

            done();
        }
        
        cleanTask(done) {
            console.log('Cleaning ' + Builder.paths.public);
            
            let deletedPaths = del.sync([
                Builder.paths.public + '/**',
                '!' + Builder.paths.public,
                '!' + Builder.paths.public + '/js',
                '!' + Builder.paths.public + '/favicon.ico',
                '!' + Builder.paths.public + '/js/bootstrap.min.js',
                '!' + Builder.paths.public + '/js/jquery.slim.min.js',
                '!' + Builder.paths.public + '/js/popper.min.js',
            ], {
                force: true
            });
            
            deletedPaths.forEach(path => {
                console.log(`Removed: ${path.replace(Builder.paths.public, '')}`);
            });
            
            done();
        }

        cssTask(done) {
            let postCssOpts = [
                advancedVariables(),
                postcssUtils(),
                precss(),
                postcssImport(),
                postcssTransformShortcut(),
                // postcssPurge({
                //     content: [
                //         Builder.paths.app + '/**/*.cshtml'
                //     ],
                //     whitelist: [
                //         'blog-figure-main',
                //         'badge-primary'
                //     ],
                //     whitelistPatterns: [
                //         /badge-.*$/gmi,
                //         /col-xl-(12|6)$/gmi
                //     ],
                //     whitelistPatternsChildren: [
                //        
                //     ],
                //     fontFace: true,
                // }),
                assets({
                    loadPaths: [Builder.paths.img.source + '/**/'],
                    baseUrl: '../'
                }),
                fontMagician({
                    hosted: [Builder.paths.font.source]
                }),
                postcssCopy({
                    basePath: [Builder.paths.source, Builder.paths.node_modules, Builder.paths.bower_components],
                    preservePath: true,
                    dest: Builder.paths.dist + '/dist',
                    transform(fileMeta) {
                        if (!['jpg', 'png', 'gif'].includes(fileMeta.ext)) {
                            return fileMeta;
                        }

                        return Imagemin.buffer(fileMeta.contents, {
                            plugins: [
                                imageminGifsicle({interlaced: true}),
                                imageminPngquant({
                                    quality: [0.7, 0.8]
                                }),
                                imageminJpegoptim({
                                    progressive: true,
                                    max: [0.7, 0.8]
                                }),
                            ]
                        })
                        .then(result => {
                            fileMeta.contents = result;
                            return fileMeta;
                        });
                    }
                }),
                cssReplace([
                    [/(\.{2}\/){2}/g, '/dist/'],
                    [/\.{2}\//g, '/dist/'],
                    [/dist\/{2}/g, ''],
                ]),
                rucksack(),
                cssnext(),
                autoprefixer({
                    remove: false
                }),
                pxToRem({
                    propList: ['*']
                }),
                // colorguard(),
                // cssgrace
            ];
            
            if (options.prod) {
                postCssOpts.push(cssnano({
                    autoprefixer: true,
                    cssDeclarationSorter: true
                }));
            }
            
            // postCssOpts.push(stylelint({
            //     "rules": {
            //         "block-no-empty": null,
            //         "color-no-invalid-hex": true,
            //         "comment-empty-line-before": [ "always", {
            //             "ignore": ["stylelint-commands", "after-comment"]
            //         } ],
            //         "declaration-colon-space-after": "always",
            //         "indentation": ["tab", {
            //             "except": ["value"]
            //         }],
            //         "max-empty-lines": 2,
            //         "rule-empty-line-before": [ "always", {
            //             "except": ["first-nested"],
            //             "ignore": ["after-comment"]
            //         } ],
            //         "unit-whitelist": ["em", "rem", "%", "s"]
            //     }
            // }));

            let files = [
                {
                    out: 'main',
                    contents: [
                        getModule('bootstrap/dist/css/bootstrap.css', 'css'),
                        getModule('Font-Awesome/css/all.css', 'css'),
                        getModule('main.scss','css'),
                    ]
                },
                {
                    out: 'index',
                    contents: [
                        getModule('index.scss', 'css'),
                    ]
                },
                {
                    out: 'about',
                    contents: [
                        getModule('about.scss', 'css'),
                    ]
                }
            ];

            let tasks = files.map((entry) => {
                entry.out += '.css';

                return gulp.src(entry.contents)
                    .pipe(sourcemaps.init({ loadMaps: true }))
                    .pipe(postcss(postCssOpts, {
                        syntax: postScss
                    }))
                    .pipe(concat(entry.out))
                    .pipe(rename({
                        extname: '.bundle.css'
                    }))
                    .pipe(sourcemaps.write('./'))
                    .pipe(gulp.dest(Builder.paths.css.dist));
            });

            es.merge.apply(null, tasks);
            done();
        }

        jsTask(done) {
            const files = [
                // {
                //     out: 'header',
                //     contents: [
                //         getModule('jquery.modernizr'),
                //     ]
                // },
                // {
                //     out: 'jquery',
                //     contents: [
                //         getModule('jquery/dist/jquery')
                //     ]
                // },
                {
                    out: 'footer',
                    contents: [
                        getModule('site'),
                        //getModule('main.ts', 'ts'),

                    ]
                },
                // {
                //     out: 'async',
                //     contents: [
                //         getModule('php/contact-form'),
                //     ]
                // }
            ];

            let tasks = files.map((entry) => {
                entry.out += '.js';

                return gulp.src(entry.contents)
                    .on('error', log.error)
                    .pipe(gulpif(/\.ts$/gmi, gulpTS({
                        
                    })))
                    .pipe(gulpif(function (file) {
                        // Pass through babel:
                        return [
                            'script'
                        ].includes(file.stem);
                    }, babel({
                        presets: ['@babel/env']
                    })))
                    .pipe(sourcemaps.init({ loadMaps: true }))
                    .pipe(deporder())
                    .pipe(concat(entry.out))
                    .pipe(rename({
                        extname: '.bundle.js'
                    }))
                    .pipe(gulpif(options.prod, stripdebug()))
                    .pipe(gulpif(options.prod, uglify()))
                    .pipe(sourcemaps.write('./'))
                    .pipe(gulp.dest(Builder.paths.js.dist));
            });



            es.merge.apply(null, tasks);
            done();
        }

        imgTask(done) {
            console.log("Doing images");
            gulp.src(Builder.paths.app + '/**/*.cshtml')
                .pipe(razor({
                    imgSource: Builder.paths.img.source,
                    imgDist: Builder.paths.img.dist
                }));

            done();
        }
    }

    new Builder();
}());
