'use strict';

import through from 'through2';
import {PluginError} from 'gulp-util';
import {JSDOM} from 'jsdom';
import jQuery from 'jquery';
import newer from "gulp-newer";
import gulp from "gulp";
import imagemin from "gulp-imagemin";
import imageminJpegoptim from 'imagemin-jpegoptim';
import reqres from 'resolve';
import path from 'path';
import imageminPngquant from "imagemin-pngquant";


export default (options = {}) => {

    const PLUGIN_NAME = 'gulp-razor';
    const REGEXP = /"~(([\/|.|\w|-])*\.(?:jpg|jpeg|png|gif))"/gmi;

    return through.obj(function (file, enc, cb) {

        if (file.isNull()) {
            this.emit('error', new PluginError(PLUGIN_NAME, 'File is null.'));
            return cb();
        }

        if (file.isStream()) {
            this.emit('error', new PluginError(PLUGIN_NAME, 'Streams are not supported!'));
            return cb();
        }

        if (file.isBuffer()) {


            // let $ = jQuery((new JSDOM(file.contents)).window);
            let files = [];

            try {

                let match;

                while ((match = REGEXP.exec(file.contents.toString())) !== null) {
                    // This is necessary to avoid infinite loops with zero-width matches
                    if (match.index === REGEXP.lastIndex) {
                        REGEXP.lastIndex++;
                    }

                    // The result can be accessed through the `match`-variable.
                    if (!files.includes(match[1])) files.push(match[1]);
                }

                // $('img').each((index, node) => {
                //     files.push($(node).attr('src').match(REGEXP)[1]);
                // });
                //
                // $('link').each((index, node) => {
                //     files.push($(node).attr('href').match(REGEXP)[1]);
                // });
                //
                // $('a').each((index, node) => {
                //     files.push($(node).attr('href').match(REGEXP)[1]);
                // });
                //
                // $('*[style]').each((index, node) => {
                //     files.push($(node).attr('style').match(REGEXP)[1]);
                // });

                for (let file of files) {
                    let dist = path.dirname(options.imgDist) + path.dirname(file);
                    let source = path.dirname(options.imgSource) + file;
                    
                    gulp.src(reqres.sync(source))
                        .pipe(newer(dist))
                        .pipe(imagemin([
                            imagemin.gifsicle({interlaced: true}),
                            imageminJpegoptim({
                                progressive: true,
                                max: [0.7, 0.8]
                            }),
                            imageminPngquant({
                                quality: [0.7, 0.8]
                            }),
                            imagemin.svgo({
                                plugins: [
                                    {removeViewBox: true},
                                    {cleanupIDs: false}
                                ]
                            })
                        ]))
                        .pipe(gulp.dest(dist));
                }

            } catch (e) {
                this.emit('error', new PluginError(PLUGIN_NAME, e));
                return cb();
            }
        }

        // make sure the file goes through the next gulp plugin
        this.push(file);

        // tell the stream engine that we are done with this file
        return cb();
    });
}
